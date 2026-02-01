using UnityEngine;
using UnityEngine.EventSystems;

namespace RefineUI
{
    public class TopPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Animator buttonAnimator;

        void Start()
        {
            buttonAnimator = this.GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
            {
                return;
            }

            else
            {
                buttonAnimator.Play("Hover");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
            {
                return;
            }

            else
            {
                buttonAnimator.Play("Normal");
            }
        }
    }
}
