﻿using Better079.Events;
using Exiled.API.Features;
using HarmonyLib;
using System;

namespace Better079
{
    public class Better079 : Plugin<Config>
    {
        public override string Author { get; } = "microsievert";
        public override Version RequiredExiledVersion { get; } = new Version("5.0.0");

        public static Better079 Instance;

        private PlayerHandlers _playerHandlers;
        private Harmony _harmonyInstance;

        public override void OnEnabled()
        {
            Instance = this;

            RegisterEvents();
            
            PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            
            UnpatchAll();

            Instance = null;

            base.OnDisabled();
        }
        
        // Patching

        private void PatchAll()
        {
            _harmonyInstance = new Harmony($"Better079Patch{DateTime.UtcNow.Ticks}");
            
            _harmonyInstance.PatchAll();
        }

        private void UnpatchAll()
        {
            _harmonyInstance.UnpatchAll();

            _harmonyInstance = null;
        }
        
        // Events

        private void RegisterEvents()
        {
            _playerHandlers = new PlayerHandlers();

            Exiled.Events.Handlers.Player.Spawning += _playerHandlers.OnSpawning;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Spawning -= _playerHandlers.OnSpawning;

            _playerHandlers = null;
        }
    }
}
