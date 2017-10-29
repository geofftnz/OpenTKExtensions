using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using OpenTK;
using OpenTKExtensions.Framework;

namespace OpenTKExtensions.Text
{
    public class TextManager : GameComponentBase, IRenderable, IResizeable
    {
        public string Name { get; set; }
        public Font Font { get; set; }
        public bool NeedsRefresh { get; private set; }
        public bool Visible { get; set; }
        public int DrawOrder { get; set; }
        public Matrix4 Projection { get; set; }
        public Matrix4 Modelview { get; set; }
        public bool AutoTransform { get; set; }

        Dictionary<string, TextBlock> blocks = new Dictionary<string, TextBlock>();
        public Dictionary<string, TextBlock> Blocks { get { return blocks; } }

        public TextManager(string name, Font font)
        {
            Name = name;
            Font = font;
            NeedsRefresh = false;
            Visible = true;
            DrawOrder = int.MaxValue;
            AutoTransform = false;
            Projection = Matrix4.Identity;
            Modelview = Matrix4.Identity;
        }

        public TextManager()
            : this("unnamed", null)
        {

        }

        public void Clear()
        {
            Blocks.Clear();
            NeedsRefresh = true;
        }

        public bool Add(TextBlock b)
        {
            if (!Blocks.ContainsKey(b.Name))
            {
                LogTrace($"TextManager.Add ({Name}): Adding \"{b.Text}\"");
                Blocks.Add(b.Name, b);
                NeedsRefresh = true;
                return true;
            }
            return false;
        }

        public void AddOrUpdate(TextBlock b)
        {
            if (!Add(b))
            {
                Blocks[b.Name] = b;
                NeedsRefresh = true;
            }
        }

        public bool Remove(string blockName)
        {
            if (Blocks.ContainsKey(blockName))
            {
                Blocks.Remove(blockName);
                NeedsRefresh = true;
                return true;
            }
            return false;
        }
        public bool RemoveAllByPrefix(string blockNamePrefix)
        {
            bool hit = false;

            foreach (var blockToRemove in Blocks.Keys.Where(n => n.StartsWith(blockNamePrefix)).ToList())
            {
                Blocks.Remove(blockToRemove);
                hit = true;
            }

            if (hit)
            {
                NeedsRefresh = true;
                return true;
            }
            return false;
        }


        public void Refresh()
        {
            LogTrace($"({Name}): Refreshing {Blocks.Count} blocks...");

            if (Font == null)
            {
                LogWarn($"({Name}): Font not specified so bailing out.");
                return;
            }

            if (!Font.IsLoaded)
            {
                LogWarn($"({Name}): Font not loaded so bailing out.");
                return;
            }

            // refresh character arrays
            Font.Clear();

            foreach (var b in Blocks.Values)
            {
                Font.AddString(b.Text, b.Position, b.Size, b.Colour);
            }

            Font.Refresh();
            NeedsRefresh = false;
        }

        public void Render(IFrameRenderData frameData)
        {
            Render();
        }
        public void Render()
        {
            if (Font == null)
            {
                LogWarn($"({Name}): Font not specified so bailing out.");
                return;
            }
            if (!Font.IsLoaded)
            {
                LogWarn($"({Name}): Font not loaded so bailing out.");
                return;
            }

            if (NeedsRefresh)
            {
                Refresh();
            }

            Font.Render(Projection, Modelview);
        }


        public void Resize(int width, int height)
        {
            if (height > 0)
            {
                Projection = Matrix4.CreateOrthographicOffCenter(0.0f, (float)width / (float)height, 1.0f, 0.0f, 0.001f, 10.0f);
                Modelview = Matrix4.Identity * Matrix4.CreateTranslation(0.0f, 0.0f, -1.0f);
            }
            else
            {
                Projection = Modelview = Matrix4.Identity;
            }
            
        }
    }
}
