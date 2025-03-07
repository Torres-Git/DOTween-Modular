using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using DOTweenModular.Enums;
using DOTweenModular.Miscellaneous;

namespace DOTweenModular
{
    /// <summary>
    /// Base class for creating DOComponents
    /// </summary>
    public abstract class DOBase : MonoBehaviour
    {
        // DO NOT USE the Tween created using these components
        // Instead use Functions provided by DOTween Modular

        #region Properties

        [Tooltip("When this tween should Begin to play" + "\n" + "\n" +
                 "OnSceneStart - Begins when this gameObject is first enabled" + "\n" + "\n" +
                 "OnVisible - Begins when visible for first time" + "\n" + "\n" +
                 "OnTrigger - Begins when gameObject defined by the Layer Mask enters trigger first time" + "\n" + "\n" +
                 "Manual - Will not Begin on its own, you have to either manually call CreateTween() & PlayTween()" + "\n" +
                           "or use this component in a DOSequence component" + "\n" + "\n" +
                 "After - Begins AFTER Tween Object's tween is completed" + "\n" + "\n" +
                 "With - Begins WITH Tween Object's tween")]
        public Begin begin;

        [Tooltip("The DO component After/With which this tween will start")]
        public DOBase tweenObject;

        [Tooltip("Layers that will start this tween")]
        public LayerMask layerMask;

        public Enums.TweenType tweenType;

        [Tooltip("Restart - Start again from start Position/Rotation/Scale" + "\n" + "\n" +
                 "Yoyo - Start from Target Position/Rotation/Scale" + "\n" + "\n" +
                 "Incremental - Continuously increments the tween at the end of each loop cycle" + "\n"+
                 "(A to B, B to B+(A-B), and so on), thus always moving 'onward'" + "\n" +
                 "In case of String tweens works only if the tween is set as relative")]
        public LoopType loopType;

        [Tooltip("Ease to apply" + "\n" + "\n" +
                 "For custom ease select INTERNAL_Custom" + "\n" + "\n" +
                 "For snapping right to target assign INTERNAL_Zero")]
        public Ease easeType;

        [Tooltip("Curve that defines Custom ease")]
        public AnimationCurve curve;

        [Tooltip("Number of loops, -1 for infinite loops " + "\n" +
                 "For Yoyo Loop Type the backward movement will also be counted")]
        [Min(-1)] public int loops = -1;

        [Tooltip("How long this tween will play")]
        [Min(0)] public float duration = 1;

        [Tooltip("Time after which this tween will play")]
        [Min(0)] public float delay;

        #endregion

        #region Events

        /// <summary>
        /// Called when this tween is created
        /// </summary>
        public UnityEvent onTweenCreated;

        /// <summary>
        /// Called the first time this tween starts
        /// </summary>
        public UnityEvent onTweenPlayed;

        /// <summary>
        /// Called every frame while this tween runs
        /// </summary>
        public UnityEvent onTweenUpdated;

        /// <summary>
        /// Called when this tween completes, in-case of infinite loops this will not invoke
        /// </summary>
        public UnityEvent onTweenCompleted;

        /// <summary>
        /// Called when this tween is Killed, in-case of infinite loops this will not invoke
        /// </summary>
        public UnityEvent onTweenKilled;

        #endregion

        /// <summary>
        /// Assign this to your custom 'Tween'.
        /// </summary>
        protected Tween Tween;

        private bool tweenPlaying;

        #region Unity Functions

        private void Awake()
        {
            if (begin == Begin.With)
                tweenObject.onTweenPlayed.AddListener(TweenObjectTween);

            if (begin == Begin.After)
                tweenObject.onTweenCompleted.AddListener(TweenObjectTween);
        }

        private void Start()
        {
            if (begin == Begin.OnSceneStart)
            {
                CreateTween();
                PlayTween();
            }
        }

        private void OnBecameVisible()
        {
            if (tweenPlaying) return;

            if (begin == Begin.OnVisible)
            {
                CreateTween();
                PlayTween();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (begin != Begin.OnTrigger) return;

            if (tweenPlaying) return;

            if (layerMask.HasLayer(other.gameObject.layer))
            {
                CreateTween();
                PlayTween();
            }
        }

        private void OnDestroy()
        {            
            if (tweenPlaying)
            {
                Tween.Kill();

                tweenPlaying = false;
            }

            curve = null;
            tweenObject = null;
            Tween = null;
        }

        #endregion

        /// <summary>
        /// Implement this method for your custom 'Tweens'
        /// </summary>
        /// <returns></returns>
        public abstract Tween CreateTween();

        /// <summary>
        /// This must be called exactly before returning the 'Tween'
        /// </summary>
        protected void TweenCreated()
        {
            OnTweenCreated();
        }

        /// <summary>
        /// Plays the Tween
        /// </summary>
        public void PlayTween()
        {
            Tween.Play();
        }

        /// <summary>
        /// Invoked by OnTweenCreated/OnTweenPlayed events of tweenObject
        /// </summary>
        private void TweenObjectTween()
        {
            CreateTween();
            PlayTween();
        }

        #region Tween Callbacks

        /// <summary>
        /// Called when Tween in Created
        /// </summary>
        protected virtual void OnTweenCreated() 
        {
            onTweenCreated?.Invoke();

            Tween.onPlay += OnTweenPlayed;
            Tween.onUpdate += OnTweenUpdate;
            Tween.onComplete += OnTweenCompleted;
            Tween.onKill += OnTweenKilled;
        }

        /// <summary>
        /// Called when Tween starts to play
        /// </summary>
        protected virtual void OnTweenPlayed()
        {
            tweenPlaying = true;

            onTweenPlayed?.Invoke();
         }

        /// <summary>
        /// Called every frame while Tween plays
        /// </summary>
        protected virtual void OnTweenUpdate() 
        {
            onTweenUpdated?.Invoke();
        }

        /// <summary>
        /// Called when the tween completes
        /// </summary>
        protected virtual void OnTweenCompleted()
        {
            onTweenCompleted?.Invoke();
            Tween.Kill();
        }

        /// <summary>
        /// Called when the tween is killed
        /// </summary>
        protected virtual void OnTweenKilled()
        {
            onTweenKilled?.Invoke();

            Tween.OnPlay(null);
            Tween.OnUpdate(null);
            Tween.OnComplete(null);
            Tween.OnKill(null);

            onTweenCreated.RemoveAllListeners();
            onTweenPlayed.RemoveAllListeners();
            onTweenUpdated.RemoveAllListeners();
            onTweenCompleted.RemoveAllListeners();
            onTweenKilled.RemoveAllListeners();
        }

        #endregion

    }
}