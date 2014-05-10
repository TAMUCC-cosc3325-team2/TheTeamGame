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


namespace TeamGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PlayerState : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Vector2 cursorPosition;
        public Texture2D cursorTexture;
        public IPuzzle puzzle;
        public Player player;
        public double status;

        public PlayerState(Game game, Player player)
            : base(game)
        {
            Game.Components.Add(this);
            this.DrawOrder = 9001;

            this.player = player;
            this.Visible = false;

            this.status = 12;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.cursorTexture = Game.Content.Load<Texture2D>("cursors/cursor");

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (player == Game1.localPlayer)
            {
                if (puzzle != null)
                    puzzle.Visible = true;
                cursorPosition = Mouse.GetState().Position(); // TODO: Mouse.SetPosition to centre of screen
                if (this.status > 0)
                    this.status -= Game1.gameDifficulty;
                else
                    this.status = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(cursorTexture, cursorPosition, player.ColorOf());
            spriteBatch.End();
        }

        public void Encode(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write((short)cursorPosition.X);
            msg.Write((short)cursorPosition.Y);
            msg.Write((byte)status);
            if (puzzle != null)
            {
                msg.Write(puzzle.ID());
                puzzle.Encode(msg);
            }
            else
                msg.Write((byte)0);
        }

        public void Decode(Lidgren.Network.NetIncomingMessage msg)
        {
            this.Visible = true;
            cursorPosition.X = msg.ReadInt16();
            cursorPosition.Y = msg.ReadInt16();
            status = msg.ReadByte();

            byte remotePuzzleType = msg.ReadByte();
            if (puzzle.ID() != remotePuzzleType)
            {
                if (puzzle != null)
                    Game.Components.Remove(puzzle);
                puzzle = remotePuzzleType.CreateFromID(Game, player);
                puzzle.Visible = true;
            }

                
            puzzle.Decode(msg);
        }
    }
}
