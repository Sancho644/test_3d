using UnityEngine;

namespace Core
{
    public class SoundSystem : MonoBehaviour
    {
        public static SoundSystem Instance { get; private set; }

        [SerializeField] private AudioClip mergeClip;
        [SerializeField] private AudioClip destroyClip;
        [SerializeField] private float volume = 1f;

        private AudioSource _source;

        private void Awake()
        {
            Instance = this;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        public void PlayMerge()
        {
            if (mergeClip != null)
                _source.PlayOneShot(mergeClip, volume);
        }

        public void PlayDestroy()
        {
            if (destroyClip != null)
                _source.PlayOneShot(destroyClip, volume);
        }
    }
}
