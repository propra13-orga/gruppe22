using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class CreateXMLFiles
    {
        public static async Task CreateXML(Mainmap _map, Camera _camera, ContentManager _content)
        {
            CreateMix(_content);
            CreateWalls(_content, "wall1");
            CreateWalls(_content, "wall2");
            CreateWalls(_content, "wall3");
            CreateFloor(_content, "floor1");
            CreateFloor(_content, "floor2");
            CreateFloor(_content, "floor3");
            CreateWalls(_content, "wall4");
            _content.Unload();
            CreateActor(_map, _content, _camera, "bat");
            CreateActor(_map, _content, _camera, "bow");
            CreateActor(_map, _content, _camera, "croc");
            CreateActor(_map, _content, _camera, "Cyclops");
            CreateActor(_map, _content, _camera, "dead");
            CreateActor(_map, _content, _camera, "devil");
            CreateActor(_map, _content, _camera, "dragon");
            CreateActor(_map, _content, _camera, "fairy");
            CreateActor(_map, _content, _camera, "Ghost");
            CreateActor(_map, _content, _camera, "gnome");
            CreateActor(_map, _content, _camera, "guard");
            CreateActor(_map, _content, _camera, "Luigi");
            CreateActor(_map, _content, _camera, "mage");
            CreateActor(_map, _content, _camera, "monk");
            CreateActor(_map, _content, _camera, "mouse");
            CreateActor(_map, _content, _camera, "Mummy");
            CreateActor(_map, _content, _camera, "Necro");
            CreateActor(_map, _content, _camera, "NPC1");
            CreateActor(_map, _content, _camera, "player");
            CreateActor(_map, _content, _camera, "princess");
            CreateActor(_map, _content, _camera, "rat");
            CreateActor(_map, _content, _camera, "skeleton");
            CreateActor(_map, _content, _camera, "skull");
            CreateActor(_map, _content, _camera, "spider");
            CreateActor(_map, _content, _camera, "thief");
            CreateActor(_map, _content, _camera, "vamp");
            CreateActor(_map, _content, _camera, "wolf");
            CreateActor(_map, _content, _camera, "xmas");
            CreateActor(_map, _content, _camera, "Zombie");
            _content.Unload();
            return;
        }

        private static void _AddActivity(ContentManager _content, ActorView actor, string character, string action, Activity activity)
        {
            if (System.IO.File.Exists(".\\content\\" + character + "-" + action + ".xnb"))
            {
                try
                {
                    Texture2D texture = _content.Load<Texture2D>(character + "-" + action);
                    int size = texture.Height / 8;
                    int cols = texture.Width / size;
                    actor.width = size;
                    actor.height = size;
                    int diff = 0;
                    if (size < 128)
                    {
                        diff = 128 - size;
                    }

                    // add offset / crop
                    actor.Add(activity, Direction.DownRight, character + "-" + action, new Coords(size * 0, size * 0), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2));
                    actor.Add(activity, Direction.UpRight, character + "-" + action, new Coords(size * 0, size * 1), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2)); // Ok
                    actor.Add(activity, Direction.Right, character + "-" + action, new Coords(size * 0, size * 2), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2)); // OK
                    actor.Add(activity, Direction.Up, character + "-" + action, new Coords(size * 0, size * 3), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2)); // Ok
                    actor.Add(activity, Direction.DownLeft, character + "-" + action, new Coords(size * 0, size * 4), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2)); // Ok
                    actor.Add(activity, Direction.Down, character + "-" + action, new Coords(size * 0, size * 5), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2));
                    actor.Add(activity, Direction.Left, character + "-" + action, new Coords(size * 0, size * 6), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2)); // OK
                    actor.Add(activity, Direction.UpLeft, character + "-" + action, new Coords(size * 0, size * 7), cols, 1,new Coords(diff/2,diff/2),new Coords(diff-diff/2,diff-diff/2));
                }
                catch
                {

                }
                _content.Unload();
            }
        }
        public static void CreateActor(Mainmap _map, ContentManager _content, Camera _camera, string character = "")
        {
            ActorView player = new ActorView(_camera, _map, 0, _content, Coords.Zero);
            _AddActivity(_content, player, character, "walk", Activity.Walk);
            _AddActivity(_content, player, character, "hit", Activity.Hit);
            _AddActivity(_content, player, character, "die", Activity.Die);
            _AddActivity(_content, player, character, "talk", Activity.Talk);
            _AddActivity(_content, player, character, "attack", Activity.Attack);
            _AddActivity(_content, player, character, "special", Activity.Special);
            player.Save("Content\\" + character + ".xml");
        }

        public static void CreateWalls(ContentManager _content, string name)
        {
            WallTiles _tiles = new WallTiles(_content, 128, 192, "");
            _tiles.Add(name + "a", WallDir.UpRight, new Rectangle(0, 768, 128, 192));
            _tiles.Add(name + "a", WallDir.LeftRightUp,
         new Rectangle(640, 596, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.LeftRightDown,
            new Rectangle(768, 596, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.UpDownLeft,
            new Rectangle(896, 596, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.UpDownRight,
            new Rectangle(512, 596, 128, 192), new Coords(0, 20));

            _tiles.Add(name + "a", WallDir.UpLeft, new Rectangle(128, 768, 128, 192));
            _tiles.Add(name + "a", WallDir.DownLeft, new Rectangle(256, 576, 128, 192));
            _tiles.Add(name + "a", WallDir.DownRight, new Rectangle(384, 576, 128, 192));
            _tiles.Add(name + "a", WallDir.LeftRight, new Rectangle(0, 584, 128, 184), new Coords(0, 8));
            _tiles.Add(name + "a", WallDir.UpDown, new Rectangle(128, 584, 128, 184), new Coords(0, 8));
            _tiles.Add(name + "a", WallDir.FourWay, new Rectangle(384, 768, 128, 192));
            _tiles.Add(name + "a", WallDir.RightClose, new Rectangle(256, 192, 128, 192));
            _tiles.Add(name + "a", WallDir.UpClose, new Rectangle(128, 212, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.LeftClose, new Rectangle(384, 192, 128, 192));
            _tiles.Add(name + "a", WallDir.DownClose, new Rectangle(0, 212, 128, 172), new Coords(0, 20));

            _tiles.Add(name + "a", WallDir.UpRightDiag,
            new Rectangle(566, 400, 20, 96), new Coords(54, 15), new Coords(54, 81));
            _tiles.Add(name + "a", WallDir.UpLeftDiag,
            new Rectangle(321, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DownLeftDiag,
            new Rectangle(384, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.DownRightDiag,
            new Rectangle(128, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.UpDownLeftDiag,
            new Rectangle(640, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.UpDownDiag,
            new Rectangle(0, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.FourDiag,
            new Rectangle(256, 768, 128, 192));
            _tiles.Add(name + "a", WallDir.RightCloseDiag,
            new Rectangle(149, 46, 22, 50), new Coords(53, 61), new Coords(53, 82)); // wrong size!
            _tiles.Add(name + "a", WallDir.UpCloseDiag,
            new Rectangle(257, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.LeftCloseDiag,
            new Rectangle(385, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DownCloseDiag,
            new Rectangle(210, 0, 30, 192), new Coords(51, 0), new Coords(49, 0)); // Wrong size!
            _tiles.Add(name + "a", WallDir.LeftRightUpDiag,
            new Rectangle(896, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.LeftRightDownDiag,
            new Rectangle(768, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.LeftRightDiag,
            new Rectangle(256, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.UpDownRightDiag,
            new Rectangle(512, 384, 128, 192));
            _tiles.Add(name + "a", WallDir.DiagUpClose,
            new Rectangle(640, 192, 128, 192));
            _tiles.Add(name + "a", WallDir.DiagDownClose,
            new Rectangle(896, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DiagUpClose2,
            new Rectangle(512, 192, 128, 192));
            _tiles.Add(name + "a", WallDir.DiagDownClose2,
            new Rectangle(768, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DiagLeftClose,
            new Rectangle(640, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DiagRightClose,
            new Rectangle(896, 192, 128, 192));
            _tiles.Add(name + "a", WallDir.DiagLeftClose2,
            new Rectangle(512, 20, 128, 172), new Coords(0, 20));
            _tiles.Add(name + "a", WallDir.DiagRightClose2,
            new Rectangle(768, 192, 128, 192));
            _tiles.Add("Column", WallDir.Free, new Rectangle(1920, 0, 128, 192));



            _tiles.Add(name + "b", WallDir.UpLeftDiag, new Rectangle(192, 0, 64, 176), new Coords(32, 16), new Coords(32, 0), WallType.OpenDoor);
            _tiles.Add(name + "b", WallDir.UpLeftDiag, new Rectangle(128, 0, 64, 176), new Coords(32, 16), new Coords(32, 0), WallType.ClosedDoor);


            _tiles.Add(name + "b", WallDir.UpDown, new Rectangle(0, 211, 128, 172), new Coords(0, 20), null, WallType.ClosedDoor);
            _tiles.Add(name + "b", WallDir.UpDown, new Rectangle(128, 211, 128, 172), new Coords(0, 20), null, WallType.OpenDoor);


            _tiles.Add(name + "b", WallDir.LeftRight, new Rectangle(384, 211, 128, 172), new Coords(0, 20), null, WallType.OpenDoor);
            _tiles.Add(name + "b", WallDir.LeftRight, new Rectangle(256, 211, 128, 172), new Coords(0,20), null, WallType.ClosedDoor);


            _tiles.Add(name + "b", WallDir.UpLeftDiag, new Rectangle(256, 0, 128, 192), new Coords(32, 16), new Coords(32, 0), WallType.Deco1);
            _tiles.Add(name + "b", WallDir.UpLeftDiag, new Rectangle(320, 0, 128, 192), new Coords(32, 16), new Coords(32, 0), WallType.Deco2);
            _tiles.Add(name + "b", WallDir.UpLeftDiag, new Rectangle(384, 0, 128, 192), new Coords(32, 16), new Coords(32, 0), WallType.Deco3);


            _tiles.Add(name + "b", WallDir.UpDown, new Rectangle(0, 576, 127, 191), null, new Coords(1, 1), WallType.Deco1);
            _tiles.Add(name + "b", WallDir.UpDown, new Rectangle(256, 576, 127, 191), null, new Coords(1, 1), WallType.Deco2);
            _tiles.Add(name + "b", WallDir.UpDown, new Rectangle(0, 768, 128, 192), null, null, WallType.Deco3);

            _tiles.Add(name + "b", WallDir.LeftRight, new Rectangle(128, 575, 128, 192), null, null, WallType.Deco1);
            _tiles.Add(name + "b", WallDir.LeftRight, new Rectangle(384, 576, 127, 191), null, new Coords(1, 1), WallType.Deco2);
            _tiles.Add(name + "b", WallDir.LeftRight, new Rectangle(128, 768, 128, 192), null, null, WallType.Deco3);

            _tiles.Save("Content\\" + name + ".xml");
        }

        public static void CreateFloor(ContentManager _content, string name)
        {
            WallTiles tmp = new WallTiles(_content, 128, 96, "");

            tmp.Add(name, WallDir.None, new Rectangle(512, 384, 128, 96));
            tmp.Save("Content\\" + name + ".xml");

        }

        public static void CreateMix(ContentManager _content)
        {

            TileSet tmp = new TileSet(_content, 64, 64);
            tmp.Add("Aniarrow", 1, new Rectangle(0, 0, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 2, new Rectangle(0, 64, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 3, new Rectangle(0, 128, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 4, new Rectangle(0, 192, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 5, new Rectangle(0, 256, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 6, new Rectangle(0, 320, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 7, new Rectangle(0, 384, 64, 64), 16, 1, false);
            tmp.Add("Aniarrow", 8, new Rectangle(0, 448, 64, 64), 16, 1, false);
            tmp.Add("spikefield", 9, new Rectangle(64, 128, 64, 64));
            tmp.Add("spikefield", 10, new Rectangle(64, 192, 64, 64));
            tmp.Add("stairs1", 11, new Rectangle(0, 717, 160, 208));
            tmp.Add("stairs1", 12, new Rectangle(160, 717, 160, 208));
            tmp.Add("stairs1", 13, new Rectangle(398, 702, 112, 208));
            tmp.Add("stairs2", 14, new Rectangle(0, 320, 192, 160));
            tmp.Add("stairs2", 15, new Rectangle(0, 480, 192, 160));
            tmp.Add("stairs2", 16, new Rectangle(192, 320, 192, 160));
            tmp.Add("stairs2", 17, new Rectangle(192, 480, 192, 160));
            tmp.Add("stairs2", 18, new Rectangle(400, 160, 112, 160));
            tmp.Add("stairs2", 19, new Rectangle(400, 480, 112, 160));
            tmp.Add("chest", 20, new Rectangle(0, 0, 64, 80), 9, 1, false);
            tmp.Add("chest", 21, new Rectangle(0, 80, 64, 80), 9, 1, false);
            tmp.Add("chest", 22, new Rectangle(0, 160, 64, 80), 9, 1, false);
            tmp.Add("fields", 23, new Rectangle(0, 0, 64, 48));
            tmp.Add("checkpoint", 24, new Rectangle(0, 0, 128, 96));
            tmp.Save("Content\\Misc.xml");


            tmp = new TileSet(_content, 64, 64);

            tmp.Add("Arrow", (int)Math.Log((double)Direction.UpRight, 2), new Rectangle(0, 0, 32, 64)); // ok
            tmp.Add("Arrow", (int)Math.Log((double)Direction.Right, 2), new Rectangle(32, 0, 32, 64));
            tmp.Add("Arrow", (int)Math.Log((double)Direction.DownRight, 2), new Rectangle(64, 0, 32, 64)); // ok
            tmp.Add("Arrow", (int)Math.Log((double)Direction.Down, 2), new Rectangle(96, 0, 32, 64));
            tmp.Add("Arrow", (int)Math.Log((double)Direction.DownLeft, 2), new Rectangle(0, 64, 32, 64)); // ok
            tmp.Add("Arrow", (int)Math.Log((double)Direction.Left, 2), new Rectangle(32, 64, 32, 64));
            tmp.Add("Arrow", (int)Math.Log((double)Direction.UpLeft, 2), new Rectangle(64, 64, 32, 64)); // ok
            tmp.Add("Arrow", (int)Math.Log((double)Direction.Up, 2), new Rectangle(96, 64, 32, 64));
            tmp.Save("Content\\Arrow.xml");

            tmp = new TileSet(_content, 64, 64);
            tmp.Add("sparks", 0, new Rectangle(0, 192, 64, 64), 1, 4);
            tmp.Add("blood", 1, new Rectangle(0, 0, 96, 96), 6, 1);
            tmp.Add("magic", 2, new Rectangle(0, 0, 64, 96), 17, 2, true);
            tmp.Save("Content\\explosion.xml");

            tmp = new TileSet(_content, 64, 64);
            tmp.Add("Shop", 0, new Rectangle(448, 192, 64, 64), 1, 1, false);
            tmp.Add("Shop", 1, new Rectangle(354, 509, 64, 96), 1, 1, false);
            tmp.Add("Shop", 2, new Rectangle(195, 256, 64, 64), 1, 1, false);
            tmp.Add("Shop", 3, new Rectangle(0, 512, 96, 128), 1, 1, false);
            tmp.Save("Content\\shop.xml");
        }
    }
}
