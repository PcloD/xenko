using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Rendering.Materials;

namespace SiliconStudio.Xenko.Rendering
{
    /// <summary>
    /// Compute and upload skinning info.
    /// </summary>
    public class SkinningRenderFeature : SubRenderFeature
    {
        private StaticEffectObjectPropertyKey<RenderEffect> renderEffectKey;

        private ObjectPropertyKey<RenderModelFrameInfo> renderModelObjectInfoKey;

        private ConstantBufferOffsetReference blendMatrices;

        private readonly FastList<NodeFrameInfo> nodeInfos = new FastList<NodeFrameInfo>();

        struct NodeFrameInfo
        {
            // Copied during Extract
            public Matrix LinkToMeshMatrix;

            public Matrix NodeTransformation;
        }

        struct RenderModelFrameInfo
        {
            public int NodeInfoOffset;

            public int NodeInfoCount;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            renderModelObjectInfoKey = RootRenderFeature.CreateObjectKey<RenderModelFrameInfo>();
            renderEffectKey = ((RootEffectRenderFeature)RootRenderFeature).RenderEffectKey;

            blendMatrices = ((RootEffectRenderFeature)RootRenderFeature).CreateDrawCBufferOffsetSlot(TransformationSkinningKeys.BlendMatrixArray.Name);
        }

        /// <inheritdoc/>
        public override void PrepareEffectPermutations()
        {
            var renderEffects = RootRenderFeature.GetData(renderEffectKey);
            int effectSlotCount = ((RootEffectRenderFeature)RootRenderFeature).EffectPermutationSlotCount;

            foreach (var renderObject in RootRenderFeature.RenderObjects)
            {
                var staticObjectNode = renderObject.StaticObjectNode;

                for (int i = 0; i < effectSlotCount; ++i)
                {
                    var staticEffectObjectNode = staticObjectNode.CreateEffectReference(effectSlotCount, i);
                    var renderEffect = renderEffects[staticEffectObjectNode];
                    var renderMesh = (RenderMesh)renderObject;

                    // Skip effects not used during this frame
                    if (renderEffect == null || !renderEffect.IsUsedDuringThisFrame(RenderSystem))
                        continue;

                    if (renderMesh.Mesh.Skinning != null)
                    {
                        renderEffect.EffectValidator.ValidateParameter(MaterialKeys.HasSkinningPosition, renderMesh.Mesh.Parameters.Get(MaterialKeys.HasSkinningPosition));
                        renderEffect.EffectValidator.ValidateParameter(MaterialKeys.HasSkinningNormal, renderMesh.Mesh.Parameters.Get(MaterialKeys.HasSkinningNormal));
                        renderEffect.EffectValidator.ValidateParameter(MaterialKeys.HasSkinningTangent, renderMesh.Mesh.Parameters.Get(MaterialKeys.HasSkinningTangent));
                        renderEffect.EffectValidator.ValidateParameter(MaterialKeys.SkinningBones, renderMesh.Mesh.Parameters.Get(MaterialKeys.SkinningBones));
                        renderEffect.EffectValidator.ValidateParameter(MaterialKeys.HasSkinningNormal, renderMesh.Mesh.Parameters.Get(MaterialKeys.HasSkinningNormal));
                        //renderEffect.EffectValidator.ValidateParameter(MaterialKeys.SkinningMaxBones, renderMesh.Mesh.Parameters.Get(MaterialKeys.SkinningMaxBones));
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Extract()
        {
            var renderModelObjectInfo = RootRenderFeature.GetData(renderModelObjectInfoKey);

            nodeInfos.Clear();

            foreach (var objectNodeReference in RootRenderFeature.ObjectNodeReferences)
            {
                var objectNode = RootRenderFeature.GetObjectNode(objectNodeReference);
                var renderMesh = (RenderMesh)objectNode.RenderObject;

                var skeleton = renderMesh.RenderModel.ModelComponent.Skeleton;
                var skinning = renderMesh.Mesh.Skinning;

                // Skip unskinned meshes
                if (skinning == null)
                {
                    renderModelObjectInfo[objectNodeReference] = new RenderModelFrameInfo();
                    continue;
                }

                var bones = skinning.Bones;
                var boneCount = bones.Length;

                // Reserve space in the node buffer
                renderModelObjectInfo[objectNodeReference] = new RenderModelFrameInfo
                {
                    NodeInfoOffset = nodeInfos.Count,
                    NodeInfoCount = boneCount
                };

                // Ensure buffer capacity
                nodeInfos.EnsureCapacity(nodeInfos.Count + boneCount);

                // Copy matrices
                for (int index = 0; index < boneCount; index++)
                {
                    var nodeIndex = bones[index].NodeIndex;

                    nodeInfos.Add(new NodeFrameInfo
                    {
                        LinkToMeshMatrix = bones[index].LinkToMeshMatrix,
                        NodeTransformation = skeleton.NodeTransformations[nodeIndex].WorldMatrix
                    });
                }
            }
        }

        /// <param name="context"></param>
        /// <inheritdoc/>
        public unsafe override void Prepare(RenderContext context)
        {
            var renderModelObjectInfoData = RootRenderFeature.GetData(renderModelObjectInfoKey);

            foreach (var renderNode in ((RootEffectRenderFeature)RootRenderFeature).renderNodes)
            {
                var blendMatricesOffset = renderNode.RenderEffect.Reflection.PerDrawLayout.GetConstantBufferOffset(blendMatrices);
                if (blendMatricesOffset == -1)
                    continue;

                var renderModelObjectInfo = renderModelObjectInfoData[renderNode.RenderObject.ObjectNode];

                var mappedCB = renderNode.Resources.ConstantBuffer.Data + blendMatricesOffset;
                var blendMatrix = (Matrix*)mappedCB;

                for (int i = 0; i < renderModelObjectInfo.NodeInfoCount; i++)
                {
                    int boneInfoIndex = renderModelObjectInfo.NodeInfoOffset + i;
                    Matrix.Multiply(ref nodeInfos.Items[boneInfoIndex].LinkToMeshMatrix, ref nodeInfos.Items[boneInfoIndex].NodeTransformation, out *blendMatrix++);
                }
            }
        }
    }
}