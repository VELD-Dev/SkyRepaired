using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyRepaired
{
    public class PluginConfig
    {
        public static PluginConfig Singleton { get; private set; }

        public readonly ConfigEntry<float> LaserDamageDelay;
        public readonly ConfigEntry<bool> LaserWorldCollision;

        public PluginConfig(ConfigFile cfg)
        {
            Singleton = this;

            LaserDamageDelay = cfg.Bind(
                "QoL Improvements",
                "Laser Damage Delay",
                0.35f,
                "Amount of time of it takes for the laser to inflict damages."
            );
            LaserWorldCollision = cfg.Bind(
                "QoL Improvements",
                "Laser World Collision",
                true,
                "Whether lasers should collide with the world or not."
            );

            Plugin.Logger.LogInfo("Config loaded !");
        }
    }
}
