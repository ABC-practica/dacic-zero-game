using HP;
using PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class WeaponSpriteHandler : MonoBehaviour
{
    [SerializeField] PlayerWeaponController WeaponController;
    [SerializeField] RawImage Image;
    [SerializeField] RawImage ImageBackground;
    [SerializeField] Slider ChargeSlider;
    [SerializeField] Image ChargeSliderFill;
    [SerializeField] private GameObject ArrowCount;


    private void Awake()
    {

    }
    private void OnEnable()
    {
        WeaponController.SwitchedActiveWeapon += updateActiveWeaponSprite;
        WeaponController.UpdateWeaponCharge += updateChargeSlider;
        ChargeSlider.maxValue = 100f;
    }

    private void OnDisable()
    {
        WeaponController.SwitchedActiveWeapon -= updateActiveWeaponSprite;
        WeaponController.UpdateWeaponCharge -= updateChargeSlider;
    }

    public void updateActiveWeaponSprite(WeaponBase weapon)
    {
        var sprite = weapon?.WeaponSprite;
        if (sprite == null)
        {
            Image.color = new Color(0, 0, 0, 0); // invis when theres no sprite, tho this shouldnt be the case in the final version.
            ImageBackground.color = new Color(0, 0, 0, 0);
            if (ArrowCount != null) ArrowCount.SetActive(false);
        }
        else if(sprite.GetType() == typeof(Bow))
        {
            Image.texture = weapon.WeaponSprite;
            Image.color = Color.white;
            ImageBackground.color = new Color(0, 0, 0, 0.5f);
            if (ArrowCount != null)
            {
                // show and sync the arrow count
                ArrowCount.SetActive(true);
                var bow = weapon as Bow;
                if (bow != null && bow.Arrows != null)
                    ArrowCount.GetComponent<TMP_Text>().text = bow.Arrows.GetComponent<TMP_Text>().text;
            }
        }
        else
        {
            Image.texture = weapon.WeaponSprite;
            Image.color = Color.white;
            ImageBackground.color = new Color(0, 0, 0, 0.5f);
            if (ArrowCount != null) ArrowCount.SetActive(false);
        }
    }

    public void updateChargeSlider(float charge)
    {
        if(charge >= 100) ChargeSliderFill.color = Color.red;
        else ChargeSliderFill.color = Color.white;
        ChargeSliderFill.color = new Color(ChargeSliderFill.color.r, // you would think i can just set the opacity
                                            ChargeSliderFill.color.g,
                                            ChargeSliderFill.color.b,
                                            0.5f
                                            );
        ChargeSlider.value = charge;
    }
}
