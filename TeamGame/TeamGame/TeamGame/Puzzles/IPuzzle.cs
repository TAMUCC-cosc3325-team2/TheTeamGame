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

        public IPuzzle(Game game, Player player)
            : base(game)
        {
            this.drawRegion = player.GetRegion();
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
    public static class IPuzzleHelper
    {
        public static Dictionary<Type, byte> map = new Dictionary<Type, byte>(){
            { typeof(Puzzles.Transition), 1},
            { typeof(Puzzles.NumeralSearch), 2},
            { typeof(Puzzles.DragCircleAvoidBlocks), 3},
            { typeof(Puzzles.TextNamesColorOfCircle), 4}
            };

        /// <summary>
        /// Returns the unique ID for the type of puzzle
        /// </summary>
        /// <param name="puzzle">The puzzle</param>
        /// <returns>0 if none, 1-255 if valid</returns>
        public static byte ID(this IPuzzle puzzle)
        {
            return map[puzzle.GetType()];
        }
        public static IPuzzle CreateFromID(this byte puzzleID, Game game, Player player)
        {
            return (IPuzzle)Activator.CreateInstance(map.FirstOrDefault(pair => pair.Value == puzzleID).Key, new object[] { game, player });
        }
    }
}