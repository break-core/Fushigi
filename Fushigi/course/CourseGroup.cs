﻿using Fushigi.Byml;
using Fushigi.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fushigi.course
{
    public class CourseGroup
    {
        public CourseGroup(BymlHashTable table, CourseActorHolder actorHolder)
        {
            mHash = BymlUtil.GetNodeData<ulong>(table["Hash"]);

            BymlArrayNode groups = table["Actors"] as BymlArrayNode;

            foreach (BymlBigDataNode<ulong> node in groups.Array)
            {
                mActors.Add(actorHolder[node.Data]);
            }
        }

        public ulong GetHash()
        {
            return mHash;
        }

        public bool IsActorValid(ulong hash)
        {
            return mActors.Any(a => a.GetHash() == hash);
        }

        public void RemoveActor(ulong hash)
        {
            mActors.RemoveAt(mActors.FindIndex(a => a.GetHash() == hash));
        }

        public BymlHashTable BuildNode()
        {
            BymlHashTable tableNode = new();
            tableNode.AddNode(BymlNodeId.UInt64, BymlUtil.CreateNode<ulong>("Hash", mHash), "Hash");

            BymlArrayNode actorsArray = new((uint)mActors.Count);

            foreach (CourseActor actor in mActors)
            {
                /* there are levels that were created in non-legit editors and that caused some things to be null when they should not have been */
                if (actor == null)
                {
                    continue;
                }
                actorsArray.AddNodeToArray(BymlUtil.CreateNode<ulong>("", actor.GetHash()));
            }

            tableNode.AddNode(BymlNodeId.Array, actorsArray, "Actors");
            return tableNode;
        }

        public List<CourseActor> GetActors()
        {
            return mActors;
        }

        ulong mHash;
        List<CourseActor> mActors = new();
    }

    public class CourseGroupHolder
    {
        public CourseGroupHolder()
        {
        
        }

        public CourseGroupHolder(BymlArrayNode array, CourseActorHolder actorHolder)
        {
            foreach (BymlHashTable tbl in array.Array)
            {
                mGroups.Add(new CourseGroup(tbl, actorHolder));
            }

        }

        CourseGroup GetGroup(ulong hash)
        {
            foreach (CourseGroup grp in mGroups)
            {
                if (grp.GetHash() == hash)
                {
                    return grp;
                }
            }

            return null;
        }

        public CourseGroup this[ulong hash]
        {
            get
            {
                return GetGroup(hash);
            }
        }

        public void RemoveFromGroup(ulong hash)
        {
            foreach (CourseGroup grp in mGroups)
            {
                if (!grp.IsActorValid(hash))
                {
                    continue;
                }

                grp.RemoveActor(hash);
            }
        }

        public BymlArrayNode SerializeToArray()
        {
            BymlArrayNode arrayNode = new((uint)mGroups.Count);

            foreach(CourseGroup grp in mGroups)
            {
                arrayNode.AddNodeToArray(grp.BuildNode());
            }

            return arrayNode;
        }

        List<CourseGroup> mGroups = new();
    }
}
