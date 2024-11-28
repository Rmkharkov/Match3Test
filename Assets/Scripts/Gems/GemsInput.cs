using Board;
using Core;
using UnityEngine;
using UnityEngine.Events;
namespace Gems
{
    public class GemsInput : MonoBoardSubscriber<GemsInput>, IGemsInput
    {
        private Camera CurrentCamera => Camera.main;
        private Vector2 firstTouchPosition;
        private Vector2 finalTouchPosition;
        private float swipeAngle;
        private bool mousePressed;

        private bool isInDefaultState;

        public UnityEvent<Vector2, Vector2> SwitchGemsInputEvent { get; } = new UnityEvent<Vector2, Vector2>();

        private void Update()
        {
            if (mousePressed && Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                OnGemFinishInput();
            }
        }

        protected override void OnChangedBoardState(EBoardState _State)
        {
            base.OnChangedBoardState(_State);
            isInDefaultState = _State == EBoardState.Default;
        }

        public void OnGemStartInput(GameObject _GemObject)
        {
            if (!isInDefaultState) return;
        
            firstTouchPosition = CurrentCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }

        private void OnGemFinishInput()
        {
            if (!isInDefaultState) return;
        
            finalTouchPosition = CurrentCamera.ScreenToWorldPoint(Input.mousePosition);
            finalTouchPosition.x = Mathf.Clamp(finalTouchPosition.x, firstTouchPosition.x - 1, firstTouchPosition.x + 1);
            finalTouchPosition.y = Mathf.Clamp(finalTouchPosition.y, firstTouchPosition.y - 1, firstTouchPosition.y + 1);
            SwitchGemsInputEvent.Invoke(firstTouchPosition, finalTouchPosition);
        }
    }
}
