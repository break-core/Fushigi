﻿using Fushigi.Byml;
using Fushigi.util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fushigi.course
{
    public class CourseLink
    {
        public CourseLink(BymlHashTable table, CourseActorHolder actorHolder)
        {
            mSource = actorHolder[BymlUtil.GetNodeData<ulong>(table["Src"])];
            mDest = actorHolder[BymlUtil.GetNodeData<ulong>(table["Dst"])];
            mLinkName = BymlUtil.GetNodeData<string>(table["Name"]);
        }

        public BymlHashTable BuildNode()
        {
            BymlHashTable tbl = new();
            tbl.AddNode(BymlNodeId.UInt64, BymlUtil.CreateNode<ulong>("Dst", mDest.GetHash()), "Dst");
            tbl.AddNode(BymlNodeId.String, BymlUtil.CreateNode<string>("Name", mLinkName), "Name");
            tbl.AddNode(BymlNodeId.UInt64, BymlUtil.CreateNode<ulong>("Src", mSource.GetHash()), "Src");
            return tbl;
        }

        public ulong GetSrcHash()
        {
            return mSource.GetHash();
        }

        public ulong GetDestHash()
        {
            return mDest.GetHash();
        }

        public void SetDestHash(ulong hash, CourseActorHolder actorHolder)
        {
            mDest = actorHolder[hash];
        }

        public bool AreLinksValid(CourseActorHolder actorHolder)
        {
            return actorHolder.HasHash(mSource.GetHash()) && actorHolder.HasHash(mDest.GetHash());
        }

        public string GetLinkName()
        {
            return mLinkName;
        }

        public bool IsSourceValid(CourseActorHolder actorHolder)
        {
            return actorHolder.HasHash(mSource.GetHash());
        }

        public bool IsDestValid(CourseActorHolder actorHolder)
        {
            return actorHolder.HasHash(mDest.GetHash());
        }

        CourseActor? mSource;
        CourseActor? mDest;
        string mLinkName;
    }

    public class CourseLinkHolder
    {
        public CourseLinkHolder()
        {

        }

        public CourseLinkHolder(BymlArrayNode linkArray, CourseActorHolder actorHolder)
        {
            foreach (BymlHashTable tbl in linkArray.Array)
            {
                mLinks.Add(new CourseLink(tbl, actorHolder));
            }
        }

        public void DeleteLinkWithDest(ulong hash)
        {
            int idx = -1;
            foreach (CourseLink link in mLinks)
            {
                if (link.GetDestHash() == hash)
                {
                    idx = mLinks.IndexOf(link);
                }
            }

            if (idx == -1)
            {
                return;
            }

            mLinks.RemoveAt(idx);
        }
        public void DeleteLinkWithSrc(ulong hash)
        {
            int idx = -1;
            foreach (CourseLink link in mLinks)
            {
                if (link.GetSrcHash() == hash)
                {
                    idx = mLinks.IndexOf(link);
                }
            }

            if (idx == -1)
            {
                return;
            }

            mLinks.RemoveAt(idx);
        }

        public Dictionary<string, List<ulong>> GetDestHashesFromSrc(ulong hash)
        {
            Dictionary<string, List<ulong>> hashes = [];

            foreach (CourseLink link in mLinks)
            {
                if (link.GetSrcHash() == hash)
                    hashes.GetOrCreate(link.GetLinkName()).Add(link.GetDestHash());
            }

            return hashes;
        }

        public Dictionary<string, List<ulong>> GetSrcHashesFromDest(ulong hash)
        {
            Dictionary<string, List<ulong>> hashes = [];

            foreach (CourseLink link in mLinks)
            {
                if (link.GetDestHash() == hash)
                    hashes.GetOrCreate(link.GetLinkName()).Add(link.GetSrcHash());
            }

            return hashes;
        }

        public CourseLink GetLinkWithDestHash(ulong hash) {
            foreach (CourseLink link in mLinks)
            {
                if (link.GetDestHash() == hash)
                {
                    return link;
                }
            }

            return null;
        }

        public CourseLink GetLinkWithSrcHash(ulong hash)
        {
            foreach (CourseLink link in mLinks)
            {
                if (link.GetSrcHash() == hash)
                {
                    return link;
                }
            }

            return null;
        }

        public List<CourseLink> GetLinks()
        {
            return mLinks;
        }

        public bool IsAnyLinkInvalid(CourseActorHolder holder)
        {
            foreach (CourseLink link in mLinks)
            {
                if (!link.IsSourceValid(holder) || !link.IsDestValid(holder))
                {
                    return true;
                }
            }

            return false;
        }

        public BymlArrayNode SerializeToArray()
        {
            BymlArrayNode node = new((uint)mLinks.Count);

            foreach(CourseLink link in mLinks)
            {
                node.AddNodeToArray(link.BuildNode());
            }

            return node;
        }

        List<CourseLink> mLinks = new();
    }
}
