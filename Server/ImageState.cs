using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Nito.Async.Sockets;
using Nito.Async;

namespace Server
{
    class ImageState
    {
        private ChildSocketState childState;

        private List<PictureBox> pictures;

        public ImageState()
        {
            pictures = new List<PictureBox>();
        }

        public ImageState(ChildSocketState css) : this()
        {
            ChildState = css;
        }

        public List<PictureBox> Pictures
        {
            get { return pictures; }
            set { pictures = value; }
        }

        public ChildSocketState ChildState
        {
            get { return childState; }
            set { childState = value; }
        }
    }
}
