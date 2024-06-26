using EndlessGame.Player;
using EndlessGame.Powerup;

namespace EndlessGame.PowerUp
{
    public class InvincibilityEffect : IPowerUpEffect
    {
        private PlayerController player;

        public InvincibilityEffect(PlayerController player)
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
