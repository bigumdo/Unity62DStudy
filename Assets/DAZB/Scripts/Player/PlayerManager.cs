using System;
using UnityEngine;
using YUI.Cores;

namespace YUI.Agents.players {
    public class PlayerManager : MonoSingleton<PlayerManager> {
        public Player Player => player;
        private Player player;

        public void SetPlayer(Player player) {
            this.player = player;
        }

        public void StartPlayerInput()
        {
            player.InputReader.Enable(true);
            player.InputReader.SetSlowMode(false);
        }

        public void StopPlayerInput()
        {
            player.InputReader.Enable(false);
            player.InputReader.SetSlowMode(true);
        }
    }
}
