﻿using System.Collections.Generic;
using MEC;
using UnityEngine;
using Exiled.API.Features;
using System.Linq;
using Exiled.API.Features.Roles;
using Camera = Exiled.API.Features.Camera;

namespace Better079.Components
{
    public class Scp079Extensions : MonoBehaviour
    {
        private Player _player;
        private ReferenceHub _playerHub;
        private Scp079Role _playerRole;

        void Start()
        {
            _playerHub = GetComponent<ReferenceHub>();
            _player = Player.Get(_playerHub);
            _playerRole = _player.Role as Scp079Role;

            if (Better079.Instance.Config.BetterMapEnabled)
               Timing.RunCoroutine(MapUpdate());
            
            if (Better079.Instance.Config.AutoTierEnabled)
                Timing.RunCoroutine(AutoTierLevelup());
        }

        private IEnumerator<float> MapUpdate()
        {
            Scp079PlayerScript scp079Script = _playerHub.scp079PlayerScript;

            yield return Timing.WaitForSeconds(1f);

            HashSet<Vector3> coordinatesPocket = new HashSet<Vector3>();

            for (;;)
            {
                foreach (Player target in Player.Get(x => x.IsAlive))
                {
                    if (target.Zone != _playerRole.Camera.Zone || target.Role.Is<Scp079Role>(out _))
                        continue;

                    coordinatesPocket.Add(target.Position);
                }

                scp079Script.TargetSetupIndicators(_playerHub.networkIdentity.connectionToClient, coordinatesPocket.ToList());

                coordinatesPocket.Clear();

                yield return Timing.WaitForSeconds(0.6f);
            }
        }

        public IEnumerator<float> CallSystemGlitch()
        {
            _playerHub.scp079PlayerScript.Mana -= Better079.Instance.Config.Scp2179EnergyLost;

            Scp079Role scp079Role = _player.Role as Scp079Role;
            
            scp079Role.Energy -= Better079.Instance.Config.Scp2179EnergyLost;
            
            for (int i = 0; i < 5; i++)
            {
                scp079Role.Camera = Camera.Random;

                yield return Timing.WaitForSeconds(1f);
            }
        }
        
        private IEnumerator<float> AutoTierLevelup()
        {
            Scp079PlayerScript playerScript = _playerHub.scp079PlayerScript;

            yield return Timing.WaitForSeconds(Better079.Instance.Config.AutoTierTime);

            playerScript.ForceLevel((byte)Better079.Instance.Config.AutoTierLevel, true);
        }
    }
}