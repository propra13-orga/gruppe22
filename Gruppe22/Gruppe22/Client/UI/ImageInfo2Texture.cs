using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{
    public class TextureFromData
    {
        public static Texture2D Convert(Backend.ImageData src,ContentManager content){
            return content.Load<Texture2D>(src.name);
        }
    }
}
