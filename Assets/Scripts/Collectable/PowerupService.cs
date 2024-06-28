using EndlessGame.Player;
using EndlessGame.PowerUp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace EndlessGame.Powerup
{
    public class PowerUpService : IPowerUpService
    {
        private ConcurrentDictionary<PowerUpType, (IPowerUpEffect, float)> activePowerUps = new ConcurrentDictionary<PowerUpType, (IPowerUpEffect, float)>();
        private Dictionary<PowerUpType, float> powerUpDurations = new Dictionary<PowerUpType, float>()
        {
            { PowerUpType.Invincibility, 5f },
            { PowerUpType.JumpBoost, 10f }
        };

        private Dictionary<PowerUpType, Func<IPlayerController, IPowerUpEffect>> powerUpCreators;

        public event Action<PowerUpType> OnPowerUpActivated;
        public event Action<PowerUpType> OnPowerUpDeactivated;

        public PowerUpService()
        {
            powerUpCreators = new Dictionary<PowerUpType, Func<IPlayerController, IPowerUpEffect>>()
            {
                { PowerUpType.Invincibility, player => new InvincibilityEffect(player) },
                { PowerUpType.JumpBoost, player => new JumpBoostEffect(player) }
            };
        }

        public void Update(float deltaTime)
        {
            List<PowerUpType> expiredPowerUps = new List<PowerUpType>();

            foreach (var powerUp in activePowerUps)
            {
                while (true)
                {
                    var currentValue = activePowerUps[powerUp.Key];
                    var updatedValue = (currentValue.Item1, currentValue.Item2 - deltaTime);

                    if (activePowerUps.TryUpdate(powerUp.Key, updatedValue, currentValue))
                    {
                        break;
                    }
                }

                if (activePowerUps[powerUp.Key].Item2 <= 0)
                {
                    expiredPowerUps.Add(powerUp.Key);
                }
            }

            foreach (var powerUp in expiredPowerUps)
            {
                DeactivatePowerUp(powerUp);
            }
        }

        public void ActivatePowerUp(PowerUpType powerUpType, PlayerController player)
        {
            if (powerUpDurations.ContainsKey(powerUpType))
            {
                if (powerUpCreators.TryGetValue(powerUpType, out var creator))
                {
                    IPowerUpEffect powerUpEffect = creator(player);
                    var duration = powerUpDurations[powerUpType];
                    activePowerUps[powerUpType] = (powerUpEffect, duration);
                    powerUpEffect.Activate();
                    OnPowerUpActivated?.Invoke(powerUpType);  // Notify activation
                }
                else
                {
                    throw new ArgumentException($"PowerUpType {powerUpType} is not supported");
                }
            }
        }

        public void DeactivatePowerUp(PowerUpType powerUpType)
        {
            if (activePowerUps.TryGetValue(powerUpType, out var powerUpEffectTuple))
            {
                powerUpEffectTuple.Item1.Deactivate();
                activePowerUps.TryRemove(powerUpType, out _); // Remove power-up atomically
                OnPowerUpDeactivated?.Invoke(powerUpType);  // Notify deactivation
            }
        }

        public bool IsPowerUpActive(PowerUpType powerUpType)
        {
            return activePowerUps.ContainsKey(powerUpType);
        }

        public IEnumerable<PowerUpType> GetActivePowerUps()
        {
            return activePowerUps.Keys.ToList(); // Return a list of active power-up types
        }
        public float GetRemainingDuration(PowerUpType powerUpType)
        {
            if (activePowerUps.TryGetValue(powerUpType, out var powerUpEffectTuple))
            {
                return powerUpEffectTuple.Item2;
            }
            return 0;
        }

        public float GetPowerUpDuration(PowerUpType powerUpType)
        {
            if (powerUpDurations.TryGetValue(powerUpType, out var duration))
            {
                return duration;
            }
            return 0;
        }
    }

    public interface IPowerUpEffect
    {
        void Activate();
        void Deactivate();
    }

    public enum PowerUpType
    {
        Invincibility,
        JumpBoost
    }

    public interface IPowerUpService
    {
        void ActivatePowerUp(PowerUpType powerUpType, PlayerController player);
        void DeactivatePowerUp(PowerUpType powerUpType);
        bool IsPowerUpActive(PowerUpType powerUpType);
        void Update(float deltaTime);
        event Action<PowerUpType> OnPowerUpActivated;
        event Action<PowerUpType> OnPowerUpDeactivated;
        IEnumerable<PowerUpType> GetActivePowerUps();
        float GetRemainingDuration(PowerUpType powerUpType);
        float GetPowerUpDuration(PowerUpType powerUpType);
    }
}
