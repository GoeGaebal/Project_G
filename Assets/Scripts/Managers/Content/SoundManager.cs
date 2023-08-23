using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private float _BgmVolume = 0.5f;
    private float _WeatherVolume = 0.5f;
    private float _EffectVolume = 0.5f;

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // MP3 Player   -> AudioSource
    // MP3 음원     -> AudioClip
    // 관객(귀)     -> AudioListener

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.Weather].loop = true;
        }
    }

    public void AudioStop(string path, Define.Sound type)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        foreach (AudioSource source in _audioSources)
        {
            if (source.clip == audioClip)
            {
                source.Stop();
                break;
            }
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
	{
        if (audioClip == null)
            return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
			if (audioSource.isPlaying)
				audioSource.Stop();

            audioSource.volume = _BgmVolume;
			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
			audioSource.Play();
		}
        else if(type == Define.Sound.Weather)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Weather];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = _WeatherVolume;
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.volume = _EffectVolume;
            audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }

    public void ChangeBGMVolume(float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
        float tempTime = audioSource.time;
        audioSource.Stop();
        _BgmVolume = volume;
        audioSource.volume = _BgmVolume;
        audioSource.time = tempTime;
        audioSource.Play();
    }

    public void ChangeWeatherVolume(float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.Weather];
        float tempTime = audioSource.time;
        audioSource.Stop();
        _WeatherVolume = volume;
        audioSource.volume = _WeatherVolume;
        audioSource.time = tempTime;
        audioSource.Play();
    }

    public void ChangeEffectVolume(float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
        float tempTime = audioSource.time;
        audioSource.Stop();
        _EffectVolume = volume;
        audioSource.volume = _EffectVolume;
        audioSource.time = tempTime;
        audioSource.Play();
    }
}
