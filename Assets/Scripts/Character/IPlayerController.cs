
namespace EndlessGame.Player
{
    public interface IPlayerController
    {
        void MovePlayer();
        void JumpPlayer();
        void SetRunningState();
        void EndSlide();

        UnityEngine.Transform GetTransform();
    }
}
