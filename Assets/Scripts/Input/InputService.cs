using UnityEngine;

namespace EndlessGame.Service
{
    public class InputService : IInputService
    {
        private KeyCode jumpKey = KeyCode.Space;
        private KeyCode slideKey = KeyCode.LeftControl;
        private KeyCode cancelJumpKey = KeyCode.S;
        public bool IsJumpPressed()
        {
            return Input.GetKeyDown(jumpKey);
        }

        public bool IsSlidePressed()
        {
            return Input.GetKeyDown(slideKey);
        }

        public bool IsJumpCancelled()
        {
            return Input.GetKeyDown(KeyCode.S);
        }
    }

    public interface IInputService
    {
        bool IsJumpPressed();
        bool IsSlidePressed();

        bool IsJumpCancelled();
    }
}
