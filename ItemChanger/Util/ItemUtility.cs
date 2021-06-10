using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class ItemUtility
    {
        public static void GiveSequentially(IEnumerable<AbstractItem> items, AbstractPlacement location, Container container, FlingType flingType, Transform transform, 
            MessageType messageType,  Action callback = null)
        {
            IEnumerator<AbstractItem> enumerator = items.GetEnumerator();
            GiveInfo info = new GiveInfo
            {
                location = location,
                container = container,
                flingType = flingType,
                transform = transform,
                callback = callback,
                messageType = messageType,
            };

            GiveRecursive(enumerator, info);
        }

        private class GiveInfo
        {
            public AbstractPlacement location;
            public Container container;
            public FlingType flingType;
            public Transform transform;
            public Action callback;
            public MessageType messageType;
        }

        private static void GiveRecursive(IEnumerator<AbstractItem> enumerator, GiveInfo info)
        {
            if (enumerator.MoveNext())
            {
                if (enumerator.Current.IsObtained())
                {
                    GiveRecursive(enumerator, info);
                }
                else if (!enumerator.Current.IsObtained())
                {
                    enumerator.Current.Give(info.location, info.container, info.flingType, info.transform, message: info.messageType, callback: _ => GiveRecursive(enumerator, info));
                }
            }
            else
            {
                info.callback?.Invoke();
            }
        }
    }
}
