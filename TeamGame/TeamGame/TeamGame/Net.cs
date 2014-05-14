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
        public TimeSpan clock;
        bool startTeamPuzzle = false;
        NetOutgoingMessage lastMessageSent;
        

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

            Game1.pStates = new Dictionary<Player, PlayerState>();
            foreach (Player p in Enum.GetValues(typeof(Player)))
                if (p != Player.None)
                    Game1.pStates.Add(p, new PlayerState(this.Game, p));

            foreach (Player p in Enum.GetValues(typeof(Player)))
                if (p != Player.None)
                {
                    Game1.pStates[p].puzzle = new Puzzles.Transition(Game, p);
                    ((Puzzles.Transition)Game1.pStates[p].puzzle).starting = true;
                }

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            client.Connect("9001.no-ip.org", 14248);

            lastMessageSent = client.CreateMessage();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            clock += gameTime.ElapsedGameTime;
            if (Game1.localPlayer != Player.None && client.ConnectionStatus == NetConnectionStatus.Connected)
                SendState();

            if (clock.TotalSeconds >= 60)
            {
                if (Game1.pStates[Game1.localPlayer].puzzle is Puzzles.TeamCirclesInOrder || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.TeamFinalTest || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.AwaitingParticipants || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.StartingGame)
                    return;
                startTeamPuzzle = true;
                clock = new TimeSpan();
                Game.Components.Remove(Game1.pStates[Game1.localPlayer].puzzle);
                Game1.pStates[Game1.localPlayer].puzzle.Dispose();
                Game1.pStates[Game1.localPlayer].puzzle = new Puzzles.TeamCirclesInOrder(Game, Game1.localPlayer);
            }

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
                        try { ParseData(msg); }
                        catch (Exception) { }
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
                else if (msg.SequenceChannel == 31) // notified of status increase
                {
                    Player remotePlayerClockwise = (Player)msg.ReadByte();
                    Game1.pStates[remotePlayerClockwise].status = 12;
                    Game1.pStates[(Player)msg.ReadByte()].score += 100;
                    if (msg.ReadBoolean() && Game1.localPlayer.TeamOf() == remotePlayerClockwise.TeamOf()) // if time to switch to team puzzle
                    {
                        if (Game1.pStates[Game1.localPlayer].puzzle is Puzzles.TeamCirclesInOrder || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.TeamFinalTest || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.AwaitingParticipants || Game1.pStates[Game1.localPlayer].puzzle is Puzzles.StartingGame)
                            return;
                        Game.Components.Remove(Game1.pStates[Game1.localPlayer].puzzle);
                        Game1.pStates[Game1.localPlayer].puzzle.Dispose();
                        Game1.pStates[Game1.localPlayer].puzzle = new Puzzles.TeamCirclesInOrder(Game, Game1.localPlayer);
                    }
                }
                else if (msg.SequenceChannel == 30) // notified of final test
                {

                }
            }
            else
                Game1.pStates[fromPlayer].Decode(msg);
        }

        public void SendState()
        {
            NetOutgoingMessage msg = client.CreateMessage(16);
            Game1.pStates[Game1.localPlayer].Encode(msg);
            if (lastMessageSent.Data != null && msg.Data.SequenceEqual(lastMessageSent.Data))
                return;

            client.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced, (int) Game1.localPlayer);
            lastMessageSent = msg;
        }

        public void NotifyStatusIncrease()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((byte)Game1.localPlayer.ClockwisePlayer());
            msg.Write((byte)Game1.localPlayer);
            msg.Write(startTeamPuzzle);
            startTeamPuzzle = false;
            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 31);
        }
    }
}
