﻿//
// PackageDependenciesNodeBuilder.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Core;

namespace MonoDevelop.PackageManagement.NodeBuilders
{
	class PackageDependenciesNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(PackageDependenciesNode); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return PackageDependenciesNode.NodeName;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var node = (PackageDependenciesNode)dataObject;
			nodeInfo.Label = node.GetLabel ();
			nodeInfo.SecondaryLabel = node.GetSecondaryLabel ();
			nodeInfo.Icon = Context.GetIcon (node.Icon);
			nodeInfo.ClosedIcon = Context.GetIcon (node.ClosedIcon);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var dependenciesNode = (PackageDependenciesNode)dataObject;
			if (dependenciesNode.LoadedDependencies) {
				treeBuilder.AddChildren (GetTargetFrameworkNodes (dataObject));
			} else {
				var placeHolderNode = new TargetFrameworkNode (dependenciesNode, GettextCatalog.GetString ("Loading..."));
				treeBuilder.AddChild (placeHolderNode);
			}
		}

		IEnumerable<TargetFrameworkNode> GetTargetFrameworkNodes (object dataObject)
		{
			var dependenciesNode = (PackageDependenciesNode)dataObject;
			return dependenciesNode.GetTargetFrameworkNodes ();
		}

		public override void OnNodeAdded (object dataObject)
		{
			var dependenciesNode = (PackageDependenciesNode)dataObject;
			dependenciesNode.PackageDependenciesChanged += OnPackageDependenciesChanged;
		}

		public override void OnNodeRemoved (object dataObject)
		{
			var dependenciesNode = (PackageDependenciesNode)dataObject;
			dependenciesNode.PackageDependenciesChanged -= OnPackageDependenciesChanged;
		}

		void OnPackageDependenciesChanged (object sender, EventArgs e)
		{
			var dependenciesNode = (PackageDependenciesNode)sender;
			ITreeBuilder builder = Context.GetTreeBuilder (dependenciesNode);
			if (builder != null) {
				builder.UpdateAll ();
			}
		}
	}
}