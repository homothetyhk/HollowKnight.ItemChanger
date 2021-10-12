using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ItemChanger
{
    public class GetItemEventArgs : EventArgs
    {
        public string ItemName { get; }
        public AbstractItem Current { get; set; }

        public GetItemEventArgs(string itemName) => this.ItemName = itemName;
    }

    public class GetLocationEventArgs : EventArgs
    {
        public string LocationName { get; }
        public AbstractLocation Current { get; set; }

        public GetLocationEventArgs(string locationName) => this.LocationName = locationName;
    }

    public class VisitStateChangedEventArgs : EventArgs
    {
        public VisitStateChangedEventArgs(AbstractPlacement placement, VisitState newFlags)
        {
            Placement = placement;
            NewFlags = newFlags;
            Orig = placement.Visited;
        }

        public AbstractPlacement Placement { get; }
        public VisitState Orig { get; }
        public VisitState NewFlags { get; }
    }

    public class GiveEventArgs : EventArgs
    {
        public GiveEventArgs(AbstractItem orig, AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Orig = orig;
            this.Item = item;
            this.Placement = placement;
            this.Info = info;
        }

        public AbstractItem Orig { get; }
        public AbstractItem Item { get; set; }
        public AbstractPlacement Placement { get; }
        public GiveInfo Info { get; set; }
    }

    public class ReadOnlyGiveEventArgs : EventArgs
    {
        private readonly GiveInfo info;

        public ReadOnlyGiveEventArgs(AbstractItem orig, AbstractItem item, AbstractPlacement placement, GiveInfo info)
        {
            this.Orig = orig;
            this.Item = item;
            this.Placement = placement;
            this.info = info;
        }

        public AbstractItem Orig { get; }
        public AbstractItem Item { get; }
        public AbstractPlacement Placement { get; }
        public string Container => info.Container;
        public FlingType Fling => info.FlingType;
        public Transform Transform => info.Transform;
        public MessageType MessageType => info.MessageType;
        public Action<AbstractItem> Callback => info.Callback;
    }

    // Encapsulate the ModHook arguments to make it easier to deal with breaking API changes.
    public class LanguageGetArgs
    {
        public readonly string convo;
        public readonly string sheet;
        public readonly string orig;
        public string current;

        public LanguageGetArgs(string convo, string sheet, string orig)
        {
            this.convo = convo;
            this.sheet = sheet;
            this.current = this.orig = orig;
        }
    }

    public class StringGetArgs
    {
        public IString Source { get; }
        public string Orig { get; }
        public string Current { get; set; }

        public StringGetArgs(IString source)
        {
            Source = source;
            Current = Orig = source.Value;
        }
    }

    public class SpriteGetArgs
    {
        public ISprite Source { get; }
        public Sprite Orig { get; }
        public Sprite Current { get; set; }

        public SpriteGetArgs(ISprite source)
        {
            Source = source;
            Current = Orig = source.Value;
        }
    }

}
