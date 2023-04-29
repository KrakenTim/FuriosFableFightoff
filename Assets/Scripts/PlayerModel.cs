using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Sprite readySprite;
    private Sprite standard;

    [SerializeField] private Image image;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerAttack = "attack";
    public float animationTimeUntilAttack = 1f;
    public float animationTimeReturn = 1f;

    public void Awake()
    {
        standard = image.sprite;
    }

    public void UpdateReady(bool value)
    {
        if(value)
        {
            image.sprite = readySprite;
            return;
        }
        image.sprite = standard;
    }

    public void StartAnimation()
    {
        animator.SetTrigger(animationTriggerAttack);
    }
}
