﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;

namespace TeamGame.Puzzles
{
    

    //-------------------------------------------
    //there will be 4 triangles drawn as follows:
    //                                           
    //              tri1                         
    //          tri2    tri3                     
    //              tri4                         
    //                                           
    // * at the start of the game, all 4         
    //   triangles will have random orientation. 
    // * player will change orientation of       
    //   triangles by clicking on them.          
    // * once the 4 triangles form a square, the 
    //   puzzle ends                             
    //------------------------------------------- 

    class CreateASquare : IPuzzle
    {
        SoundEffectInstance prompt;
        Texture2D triangle;

        Triangle tri1, tri2, tri3, tri4;


        int randNum, countUpdates;

        MouseState mouse;
        MouseState prevMouse;

        List<Triangle> Triangles;

        /// <summary>
        /// Individual puzzle. Create a square from 4 triangles.
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        public CreateASquare(Game game, Player player)
            : base(game, player)
        {
            Triangles = new List<Triangle>();
            countUpdates = 0;

            tri1 = new Triangle();
            tri2 = new Triangle();
            tri3 = new Triangle();
            tri4 = new Triangle();

            tri1.Location = new Rectangle(50, 0, 150, 75);
            tri2.Location = new Rectangle(50, 75, 150, 75);
            tri3.Location = new Rectangle(0, 75, 150, 75);
            tri4.Location = new Rectangle(150, 75, 150, 75);

            //Are you proud of me yet?
            tri1.Rot = (randNum = Game1.random.Next(0,2)) == 0 ? Orientation.Up : randNum == 1 ? Orientation.Left : Orientation.Right;
            tri2.Rot = (randNum = Game1.random.Next(0,2)) == 0 ? Orientation.Left : randNum == 1 ? Orientation.Up : Orientation.Down;
            tri3.Rot = (randNum = Game1.random.Next(0,2)) == 0 ? Orientation.Right : randNum == 1 ? Orientation.Up : Orientation.Down;
            tri4.Rot = (randNum = Game1.random.Next(0,2)) == 0 ? Orientation.Down : randNum == 1 ? Orientation.Left : Orientation.Right;

            tri1.Stat = Status.Waiting;
            tri2.Stat = Status.Waiting;
            tri3.Stat = Status.Waiting;
            tri4.Stat = Status.Waiting;

            Triangles.Add(tri1);
            Triangles.Add(tri2);
            Triangles.Add(tri3);
            Triangles.Add(tri4);
        }

        public override void Initialize()
        {
            
            triangle = Game.Content.Load<Texture2D>("art/triangle");
            if (player == Game1.localPlayer)
            {
                //prompt = Game.Content.Load<SoundEffect>("audio/CreateASquare").CreateInstance();
                //prompt.Play();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime); // draw healthbar

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
            foreach (Triangle tri in Triangles)
                spriteBatch.Draw(triangle, tri.Location, tri.Location, player.ColorOf(), MathHelper.ToRadians(tri.Degrees), tri.Location.Location.ToVector2(), SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner
            countUpdates++;

            mouse = Mouse.GetState();

            foreach (Triangle tri in Triangles)
            {
                if(tri.Location.Contains(mouse.PPosition()))
                {
                    if(prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && tri.Stat == Status.Waiting)
                    {
                        tri.Stat = Status.Rotating;
                    }
                }
            }

            foreach (Triangle tri in Triangles)
            {
                if (tri.Stat == Status.Waiting)
                {
                    switch (tri.Rot)
                    {
                        case Orientation.Up:
                            tri.Degrees = 90;
                            break;
                        case Orientation.Left:
                            tri.Degrees = 180;
                            break;
                        case Orientation.Right:
                            tri.Degrees = 0;
                            break;
                        case Orientation.Down:
                            tri.Degrees = 270;
                            break;
                    }
                }
                else if (tri.Stat == Status.Rotating)
                {
                    tri.Degrees++;
                    if (tri.Degrees % 90 == 0)
                    {
                        tri.Stat = Status.Waiting;
                        switch (tri.Degrees)
                        {
                            case 0:
                                tri.Rot = Orientation.Right;
                                break;
                            case 90:
                                tri.Rot = Orientation.Up;
                                break;
                            case 180:
                                tri.Rot = Orientation.Left;
                                break;
                            case 270:
                                tri.Rot = Orientation.Down;
                                break;
                            case 360:
                                tri.Degrees = 0;
                                tri.Rot = Orientation.Right;
                                break;
                        }
                    }
                }
                if (tri1.Rot == Orientation.Down && tri2.Rot == Orientation.Right && tri3.Rot == Orientation.Left && tri4.Rot == Orientation.Up)
                    PuzzleOver(true);
                else if (countUpdates > 420)
                    PuzzleOver(false);

            }

            prevMouse = mouse;
        }

        public new void PuzzleOver(bool p)
        {
            //if (prompt.State == SoundState.Playing)
            //    prompt.Dispose();
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);


            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player, p);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write((short)tri1.Location.X);
            msg.Write((short)tri1.Location.Y);
            msg.Write((short)tri1.Degrees);
            msg.Write((short)tri2.Location.X);
            msg.Write((short)tri2.Location.Y);
            msg.Write((short)tri2.Degrees);
            msg.Write((short)tri3.Location.X);
            msg.Write((short)tri3.Location.Y);
            msg.Write((short)tri3.Degrees);
            msg.Write((short)tri4.Location.X);
            msg.Write((short)tri4.Location.Y);
            msg.Write((short)tri4.Degrees);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            tri1.Location = new Rectangle(msg.ReadInt16(), msg.ReadInt16(), 150, 75);
            tri1.Degrees = msg.ReadInt16();
            tri2.Location = new Rectangle(msg.ReadInt16(), msg.ReadInt16(), 150, 75);
            tri2.Degrees = msg.ReadInt16();
            tri3.Location = new Rectangle(msg.ReadInt16(), msg.ReadInt16(), 150, 75);
            tri3.Degrees = msg.ReadInt16();
            tri4.Location = new Rectangle(msg.ReadInt16(), msg.ReadInt16(), 150, 75);
            tri4.Degrees = msg.ReadInt16();
        }
    }
}