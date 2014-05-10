using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame.Puzzles
{
    public enum BlockState
    {
        Waiting, 
        MovingLeft,
        MovingRight
    }

    public enum ClickedState
    {
        Clicked,
        Released
    }

    class DragCircleAvoidBlocks : IPuzzle
    {
        int blockMoveSpeed = 2;

        Rectangle backgroundRec, ball1Rec, ball2Rec, block1Rec, block2Rec;
        Texture2D background, ball, block1, block2;

        SoundEffectInstance prompt;

        Rectangle goalRec;
        MouseState mouse;

        public ClickedState ball1State
        {
            get;
            protected set;
        }

        public ClickedState ball2State
        {
            get;
            protected set;
        }

        public BlockState block1State
        {
            get;
            protected set;
        }

        public BlockState block2State
        {
            get;
            protected set;
        }

        public DragCircleAvoidBlocks(Game game, Player player)
            : base(game, player)
        {
            backgroundRec = new Rectangle(0, 0, 250, 150);
            goalRec = new Rectangle(90, 128, 22, 22);
            ball1Rec = new Rectangle(4, 0, 22, 22);
            ball2Rec = new Rectangle(223, 128, 18, 18);
            block1Rec = new Rectangle(180, 45, 70, 17);
            block2Rec = new Rectangle(200, 83, 50, 17);
            block1State = block2State = BlockState.Waiting;
            ball1State = ball2State = ClickedState.Released;
        }

        public override void Initialize()
        {

            background = Game.Content.Load<Texture2D>("art/dragCircleTest");
            ball = Game.Content.Load<Texture2D>("art/dragableBall");
            block1 = Game.Content.Load<Texture2D>("art/avoidBlock");
            block2 = Game.Content.Load<Texture2D>("art/avoidBlock");

            if (player == Game1.localPlayer)
            {
                prompt = Game.Content.Load<SoundEffect>("audio/DragTheCircles").CreateInstance();
                prompt.Play();
            }

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
            spriteBatch.Draw(background, backgroundRec, player.ColorOf());
            spriteBatch.Draw(block1, block1Rec, player.ColorOf());
            spriteBatch.Draw(block2, block2Rec, player.ColorOf());
            spriteBatch.Draw(ball, ball1Rec, player.ColorOf());
            spriteBatch.Draw(ball, ball2Rec, player.ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner

            mouse = Mouse.GetState();

            Vector2 relativePos = Mouse.GetState().Position() - (drawRegion.Location.ToVector2());

            if (Game1.random.Next(0, 100) % 25 == 0)
            {
                if (Game1.random.Next(0, 1) == 0 && block1State == BlockState.Waiting)
                {
                    if(block1Rec.Right == backgroundRec.Right)
                        block1State = BlockState.MovingLeft;
                    else
                        block1State = BlockState.MovingRight;
                }
                else if (block2State == BlockState.Waiting)
                {
                    if(block2Rec.Right == backgroundRec.Right)
                        block2State = BlockState.MovingLeft;
                    else
                        block2State = BlockState.MovingRight;
                }
            }

            if (block1State == BlockState.MovingLeft)
            {
                if (block1Rec.Left <= backgroundRec.Left)
                {
                    block1Rec.X = backgroundRec.Left;
                    block1State = BlockState.Waiting;
                }
                else
                    block1Rec.X -= blockMoveSpeed;
            }
            else if (block1State == BlockState.MovingRight)
            {
                if (block1Rec.Right >= backgroundRec.Right)
                {
                    block1Rec.X = backgroundRec.Right - block1Rec.Width;
                    block1State = BlockState.Waiting;
                }
                else
                    block1Rec.X += blockMoveSpeed;
            }
            if (block2State == BlockState.MovingLeft)
            {
                if (block2Rec.Left <= backgroundRec.Left)
                {
                    block2Rec.X = backgroundRec.Left;
                    block2State = BlockState.Waiting;
                }
                else
                    block2Rec.X -= blockMoveSpeed;
            }
            else if (block2State >= BlockState.MovingRight)
            {
                if (block2Rec.Right == backgroundRec.Right)
                {
                    block2Rec.X = backgroundRec.Right - block2Rec.Width;
                    block2State = BlockState.Waiting;
                }
                else
                    block2Rec.X += blockMoveSpeed;
            }


            if (mouse.LeftButton == ButtonState.Pressed && ball1Rec.Contains(relativePos))
                ball1State = ClickedState.Clicked;
            else if (mouse.LeftButton == ButtonState.Pressed && ball2Rec.Contains(relativePos))
                ball2State = ClickedState.Clicked;

            if (ball1State == ClickedState.Clicked)
            {
                ball1Rec.Location = new Point((int)relativePos.X - 9, (int)relativePos.Y - 9);

                if (ball1Rec.Bottom >= backgroundRec.Bottom)
                    ball1Rec.Y = backgroundRec.Bottom - 18;
                else if (ball1Rec.Top <= backgroundRec.Top)
                    ball1Rec.Y = backgroundRec.Top;
                if (ball1Rec.Right >= backgroundRec.Right)
                    ball1Rec.X = backgroundRec.Right - 18;
                else if (ball1Rec.Left <= backgroundRec.Left)
                    ball1Rec.X = backgroundRec.Left;

                ball2Rec.X = backgroundRec.Right - 18 - ball1Rec.X;
                ball2Rec.Y = backgroundRec.Bottom - 18 - ball1Rec.Y;

                if (mouse.LeftButton == ButtonState.Released)
                    ball1State = ClickedState.Released;
            }

            else if (ball2State == ClickedState.Clicked)
            {
                ball2Rec.Location = new Point((int)relativePos.X - 9, (int)relativePos.Y - 9);

                if (ball2Rec.Bottom >= backgroundRec.Bottom)
                    ball2Rec.Y = backgroundRec.Bottom - 18;
                else if (ball2Rec.Top <= backgroundRec.Top)
                    ball2Rec.Y = backgroundRec.Top;
                if (ball2Rec.Right >= backgroundRec.Right)
                    ball2Rec.X = backgroundRec.Right - 18;
                else if (ball2Rec.Left <= backgroundRec.Left)
                    ball2Rec.X = backgroundRec.Left;

                ball1Rec.X = backgroundRec.Right - 18 - ball2Rec.X;
                ball1Rec.Y = backgroundRec.Bottom - 18 - ball2Rec.Y;

                if (mouse.LeftButton == ButtonState.Released)
                    ball2State = ClickedState.Released;
            }

            if (block1Rec.Intersects(ball1Rec.Center.X, ball1Rec.Center.Y, ball1Rec.Height/2) || block1Rec.Intersects(ball2Rec))
                PuzzleOver(false);
            else if (block2Rec.Intersects(ball1Rec) || block2Rec.Intersects(ball2Rec))
                PuzzleOver(false);

            if ((ball1Rec.Center.ToVector2() - goalRec.Center.ToVector2()).LengthSquared() <= 9)
                PuzzleOver(true);

        }

        public new void PuzzleOver(bool p)
        {
            if (prompt.State == SoundState.Playing)
                prompt.Dispose();
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);
            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player);  
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write((short)ball1Rec.X);
            msg.Write((short)ball1Rec.Y);
            //msg.Write((short)ball2Pos.X); // can be computed from the ball1Pos
            //msg.Write((short)ball2Pos.Y);
            msg.Write((short)block1Rec.X);
            msg.Write((short)block1Rec.Y);
            msg.Write((short)block2Rec.X);
            msg.Write((short)block2Rec.Y);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            ball1Rec.X = msg.ReadInt16();
            ball1Rec.Y = msg.ReadInt16();
            //ball2Pos.X = msg.ReadInt16();
            //ball2Pos.Y = msg.ReadInt16();
            block1Rec.X = msg.ReadInt16();
            block1Rec.Y = msg.ReadInt16();
            block2Rec.X = msg.ReadInt16();
            block2Rec.Y = msg.ReadInt16();

        }
    }
}
