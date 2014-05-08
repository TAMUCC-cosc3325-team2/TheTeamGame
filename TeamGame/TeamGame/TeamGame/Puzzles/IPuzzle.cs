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
    /// Interface for an individual puzzle controlled and updated by a PlayerState.
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IPuzzle : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Rectangle drawRegion;
        public Player player;
        public Matrix matrix { get { return Matrix.CreateTranslation(drawRegion.Location.X, drawRegion.Location.Y, 0); } }

        public IPuzzle(Game game, Player player, Rectangle drawRegion)
            : base(game)
        {
            this.drawRegion = drawRegion;
            this.player = player;

            game.Components.Add(this);
        }

        

        public override void Initialize() {}

        public override void Update(GameTime gameTime) { throw new NotImplementedException(); }

        public override void Draw(GameTime gameTime) { throw new NotImplementedException(); }

        public virtual void Encode(NetOutgoingMessage msg) { throw new NotImplementedException(); }

        public virtual void Decode(NetIncomingMessage msg) { throw new NotImplementedException(); }

        public virtual void PuzzleOver(bool Correct) {}
    }
}
