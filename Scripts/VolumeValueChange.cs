using UnityEngine;
using UnityEngine.UI;

public class VolumeValueChange : MonoBehaviour
{
    [SerializeField] Slider slider;
    void Awake()
    {
        if(PlayerPrefs.HasKey("Volume"))
        {
            SetVolume(PlayerPrefs.GetFloat("Volume"));
            slider.value = PlayerPrefs.GetFloat("Volume");
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}