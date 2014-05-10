using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;


namespace TeamGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Net : Microsoft.Xna.Framework.GameComponent
    {
        public NetClient client;
        public Dictionary<Player, PlayerState> pStates;

        public Net(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(Net), this);

            NetPeerConfiguration config = new NetPeerConfiguration("TeamGame");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(IncomingMessage));
            client.Start();

            pStates = new Dictionary<Player, PlayerState>();
            foreach (Player p in Enum.GetValues(typeof(Player)))
                pStates.Add(p, new PlayerState(this.Game, p));

            foreach (Player p in Enum.GetValues(typeof(Player)))
                if (p != Player.None)
                    pStates[p].puzzle = new Puzzles.Transition(Game, p);
            //pStates[Player.t1p1].puzzle = new Puzzles.NumeralSearch(Game, Player.t1p1);
            //pStates[Player.t1p2].puzzle = new Puzzles.NumeralSearch(Game, Player.t1p2);
            //pStates[Player.t1p3].puzzle = new Puzzles.NumeralSearch(Game, Player.t1p3);
            //pStates[Player.t1p4].puzzle = new Puzzles.NumeralSearch(Game, Player.t1p4);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            client.Connect("9001.no-ip.org", 14248);

            

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
                SendState();

            base.Update(gameTime);
        }

        public void IncomingMessage(object peer)
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        break;

                    case NetIncomingMessageType.Data:
                        ParseData(msg);
                        break;
                }
            }
            //client.Recycle(msg);
        }

        public void ParseData(NetIncomingMessage msg)
        {
            Player fromPlayer = (Player)msg.SequenceChannel;
            if (!(fromPlayer > 0 && fromPlayer < (Player)17 && !fromPlayer.IsLocal()))
            {
                if (msg.SequenceChannel == 20)
                {
                    Game1.localPlayer = (Player)msg.ReadByte();
                    // pStates[Game1.localPlayer].Visible = true;
                    Game.IsMouseVisible = true;
                    System.Windows.Forms.Cursor myCursor = Extensions.LoadCustomCursor("Content/cursors/cursor" + Game1.localPlayer.ColorName() + ".cur");
                    System.Windows.Forms.Form winForm = ((System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Game.Window.Handle));
                    winForm.Cursor = myCursor;
                }
            }
            else
                pStates[fromPlayer].Decode(msg);
        }

        public void SendState()
        {
            NetOutgoingMessage msg = client.CreateMessage(16);
            pStates[Game1.localPlayer].Encode(msg);
            client.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced, (int) Game1.localPlayer);
        }
    }
}