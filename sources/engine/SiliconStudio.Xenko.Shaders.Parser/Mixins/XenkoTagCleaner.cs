// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using SiliconStudio.Shaders.Ast.Xenko;
using SiliconStudio.Shaders.Ast;
using SiliconStudio.Shaders.Visitor;

namespace SiliconStudio.Xenko.Shaders.Parser.Mixins
{
    internal class XenkoTagCleaner : ShaderWalker
    {
        public XenkoTagCleaner()
            : base(false, false)
        {
        }

        public void Run(ShaderClassType shader)
        {
            Visit(shader);
        }

        public override void DefaultVisit(Node node)
        {
            // Keeping it for ShaderLinker (removed by XenkoShaderCleaner)
            //node.RemoveTag(XenkoTags.ConstantBuffer);
            node.RemoveTag(XenkoTags.ShaderScope);
            node.RemoveTag(XenkoTags.StaticRef);
            node.RemoveTag(XenkoTags.ExternRef);
            node.RemoveTag(XenkoTags.StageInitRef);
            node.RemoveTag(XenkoTags.CurrentShader);
            node.RemoveTag(XenkoTags.VirtualTableReference);
            node.RemoveTag(XenkoTags.BaseDeclarationMixin);
            node.RemoveTag(XenkoTags.ShaderScope);
            base.DefaultVisit(node);
        }
    }
}
