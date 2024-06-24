using UnityEngine;

namespace EndlessGame.Service
{
    public class InputService : IInputService
    {
        private KeyCode jumpKey = KeyCode.Space;
        private KeyCode slideKey = KeyCode.S;
        public bool IsJumpPressed()
        {
            return Input.GetKeyDown(jumpKey);
        }

        public bool IsSlidePressed()
        {
            return Input.GetKeyDown(slideKey);
        }
    }

    public interface IInputService
    {
        bool IsJumpPressed();
        bool IsSlidePressed();
    }
}
