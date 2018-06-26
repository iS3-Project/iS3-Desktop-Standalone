using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Data;

namespace IS3.Core
{
    #region Copyright Notice
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    #endregion

    // Summary:
    //     Tree class is comprised of recursive Tree-Nodes.
    //     Tree is used to provide a tree-like view of objects.
    // Remarks:
    //     Each tree node has a DataView which is linked a DataTable.
    //     Filter and Sort strings could be applied to get the data view.
    //
    [DataContract(IsReference=true)]
    public class Tree
    {
        // Name of the tree-node, and most importantly,
        // it is used as category name.
        [DataMember]
        public string Name { get; set;}

        // Display name of the tree-node, this is appeared in the window
        [DataMember]
        public string DisplayName { get; set; }

        // Description of the tree-node
        [DataMember]
        public string Description { get; set; }

        // reference domain name
        public string RefDomainName { get; set; }

        // reference objects name
        public string RefObjsName { get; set; }

        // a filter string that apply on the reference objects
        public string Filter { get; set; }

        // a sort string that apply on the reference objects
        public string Sort { get; set; }

        // The filtered and sorted view of reference objects.
        public DataView ObjectsView { get; set; }

        // get the reference DGObjects, i.e., the whole object collection
        public DGObjects RefObjs { get; set; }

        // Child Tree-Nodes
        [DataMember]
        public ObservableCollection<Tree> Children { get; set; }

        public Tree()
        {
            Children = new ObservableCollection<Tree>();
        }

        public List<Tree> ToList()
        {
            List<Tree> results = new List<Tree>();
            results.Add(this);
            foreach (Tree tree in Children)
            {
                List<Tree> childResults = tree.ToList();
                if (childResults != null && childResults.Count > 0)
                    results.AddRange(childResults);
            }
            return results;
        }

        public static Tree ParseTree(string xml)
        {
            if (xml == null)
                return null;

            XDocument doc = XDocument.Parse(xml);
            XElement root = doc.Root;
            Tree tree_root = Element2Tree(root);

            return tree_root;
        }

        public static Tree Element2Tree(XElement el)
        {
            Tree tree = new Tree();
            tree.Name = el.Name.ToString();
            if (el.HasAttributes)
            {
                XAttribute attr = el.Attribute("DisplayName");
                if (attr != null)
                    tree.DisplayName = attr.Value;

                attr = el.Attribute("Desc");
                if (attr != null)
                    tree.Description = attr.Value;

                attr = el.Attribute("RefDomainName");
                if (attr != null)
                    tree.RefDomainName = attr.Value;

                attr = el.Attribute("RefObjsName");
                if (attr != null)
                    tree.RefObjsName = attr.Value;

                attr = el.Attribute("Filter");
                if (attr != null)
                    tree.Filter = attr.Value;

                attr = el.Attribute("Sort");
                if (attr != null)
                    tree.Sort = attr.Value;
            }

            foreach (XElement childEl in el.Elements())
            {
                Tree childTree = Element2Tree(childEl);
                tree.Children.Add(childTree);
            }

            return tree;
        }

        public static XElement Tree2Element(Tree tree)
        {
            List<XAttribute> atts = new List<XAttribute>();
            if (tree.DisplayName != null)
                atts.Add(new XAttribute("DisplayName", tree.DisplayName));
            if (tree.Description != null)
                atts.Add(new XAttribute("Desc", tree.Description));
            if (tree.RefDomainName != null)
                atts.Add(new XAttribute("RefDomainName", tree.RefDomainName));
            if (tree.RefObjsName != null)
                atts.Add(new XAttribute("RefObjsName", tree.RefObjsName));
            if (tree.Filter != null)
                atts.Add(new XAttribute("Filter", tree.Filter));
            if (tree.Sort != null)
                atts.Add(new XAttribute("Sort", tree.Sort));

            XElement xe = new XElement(tree.Name, atts.ToArray());

            foreach (Tree childTree in tree.Children)
            {
                XElement childXe = Tree2Element(childTree);
                xe.Add(childXe);
            }

            return xe;
        }

        public Tree FindTree(string name)
        {
            Tree result = null;
            if (this.Name == name)
                return this;

            foreach (Tree tree in Children)
            {
                result = tree.FindTree(name);
                if (result != null)
                    break;
            }

            return result;
        }

        // Find the parent of the leaf in the root
        //
        public static Tree FindParent(Tree root, Tree leaf)
        {
            if (root.Children.Contains(leaf))
                return root;

            foreach (Tree tree in root.Children)
            {
                Tree result = FindParent(tree, leaf);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
