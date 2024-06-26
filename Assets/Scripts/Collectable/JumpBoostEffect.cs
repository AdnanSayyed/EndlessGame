using EndlessGame.Player;

namespace EndlessGame.Powerup
{
    public class JumpBoostEffect : IPowerUpEffect
    {
        private IPlayerController player;

        public JumpBoostEffect(IPlayerController player)
        {
            this.player = player;
        }

        public void Activate()
        {
            player.SetJumpBoostActive(true); 
        }

        public void Deactivate()
        {
            player.SetJumpBoostActive(false);
        }
    }
}
