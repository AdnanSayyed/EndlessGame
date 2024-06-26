using EndlessGame.Player;
using EndlessGame.Powerup;

namespace EndlessGame.PowerUp
{
    public class InvincibilityEffect : IPowerUpEffect
    {
        private IPlayerController player;

        public InvincibilityEffect(IPlayerController player)
        {
            this.player = player;
        }

        public void Activate()
        {
            player.SetInvincible(true);
        }

        public void Deactivate()
        {
            player.SetInvincible(false);
        }
    }
}
