using System.IO;
using System.Numerics;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using TinyRenderer.Shaders;
using TinyRenderer.Utils;
using static TinyRenderer.Utils.MathUtils;

namespace TinyRenderer
{
    public static class Program
    {
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 800;

        private static void Main()
        {
            var screen = new Image<Rgba32>(ScreenWidth, ScreenHeight).Fill(Rgba32.Black);

            var zBuffer = new float[ScreenWidth, ScreenHeight];

            //screen.RenderFlatShader(zBuffer);

            //screen.RenderFlatTexturedShader(zBuffer);

            //screen.RenderFlatPerspectiveShader(zBuffer);

            //screen.RenderZBufferShader(zBuffer);

            //screen.RenderGouraudShader(zBuffer);

            //screen.GouraudToonShader(zBuffer);

            //screen.RenderGouraudTexturedShader(zBuffer);

            //screen.RenderNormalMapShader(zBuffer);

            screen.RenderSpecularMapShader(zBuffer);

            //screen.RenderTangentNormalMapShader(zBuffer);

            //screen.RenderShadowShader(zBuffer);

            //screen.RenderAmbientOccolusion(zBuffer);

            //screen.RenderZBuffer(zBuffer, ScreenWidth, ScreenHeight);

            using (var output = File.OpenWrite("Render.png"))
            {
                screen.Flip(FlipType.Vertical).Save(output, new PngEncoder());
            }

            //GenerateOccolusionMap();
        }

        private static void RenderFlatShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var viewPort = CreateViewPort(0, 0, ScreenWidth, ScreenHeight);
            var light = new Vector3(0, 0, -1).Normalize();

            var shader = new FlatShader
            {
                Transform = viewPort,
                Light = light
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderFlatTexturedShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var viewPort = CreateViewPort(0, 0, ScreenWidth, ScreenHeight);
            var light = new Vector3(0, 0, -1).Normalize();

            var texture = TextureUtils.LoadTexture(@"Resources\Head\Diffuse.png");

            var shader = new FlatTexturedShader
            {
                Transform = viewPort,
                Light = light,

                Texture = texture
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderFlatPerspectiveShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(0, 0, 3);

            var viewPort = CreateViewPort(0, 0, ScreenWidth, ScreenHeight);
            var projection = CreateProjection(-1 / cameraPosition.Z);
            var light = new Vector3(0, 0, -1).Normalize();

            var shader = new FlatShader
            {
                Transform = viewPort * projection,
                Light = light
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderZBufferShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);

            var shader = new ZBufferShader
            {
                Transform = viewPort * projection * modelView
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);

            model = ModelUtils.LoadModel(@"Resources\Floor\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderGouraudShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1).Normalize();

            var shader = new GouraudShader
            {
                Transform = viewPort * projection * modelView,
                Light = light
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void GouraudToonShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1).Normalize();

            var shader = new GouraudToonShader
            {
                Transform = viewPort * projection * modelView,
                Light = light
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderGouraudTexturedShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1).Normalize();

            var texture = TextureUtils.LoadTexture(@"Resources\Head\Diffuse.png");

            var shader = new GouraudTexturedShader
            {
                Transform = viewPort * projection * modelView,
                Light = light,

                Texture = texture
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderNormalMapShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1);

            var texture = TextureUtils.LoadTexture(@"Resources\Head\Diffuse.png");
            var normalMap = TextureUtils.LoadTexture(@"Resources\Head\Normal.png");

            var shader = new NormalMapShader
            {
                Transform = viewPort * projection * modelView,
                MIT = (projection * modelView).Invert().Transpose(),
                Light = (projection * modelView).Multiply(light).Normalize(),

                Texture = texture,
                NormalMap = normalMap
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderSpecularMapShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1);

            //var texture = TextureUtils.LoadTexture(@"Resources\Head\Diffuse.png");
            //var normalMap = TextureUtils.LoadTexture(@"Resources\Head\Normal.png");
            //var specularMap = TextureUtils.LoadTexture(@"Resources\Head\Specular.png").Flip(FlipType.Vertical);

            var texture = TextureUtils.LoadTexture(@"Resources\Diablo3\Diffuse.png");
            var normalMap = TextureUtils.LoadTexture(@"Resources\Diablo3\Normal.png");
            var specularMap = TextureUtils.LoadTexture(@"Resources\Diablo3\Specular.png").Flip(FlipType.Vertical);

            var shader = new SpecularMapShader
            {
                Transform = viewPort * projection * modelView,
                MIT = (projection * modelView).Invert().Transpose(),
                Light = (projection * modelView).Multiply(light).Normalize(),

                Texture = texture,
                NormalMap = normalMap,
                SpecularMap = specularMap
            };

            //var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            var model = ModelUtils.LoadModel(@"Resources\Diablo3\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderTangentNormalMapShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1);

            var texture = TextureUtils.LoadTexture(@"Resources\Head\Diffuse.png");
            var normalMap = TextureUtils.LoadTexture(@"Resources\Head\NormalTangent.png");

            var shader = new TangentNormalMapShader
            {
                Transform = viewPort * projection * modelView,
                MIT = (projection * modelView).Invert().Transpose(),
                Light = (projection * modelView).Multiply(light).Normalize(),

                Texture = texture,
                NormalMap = normalMap
            };

            var model = ModelUtils.LoadModel(@"Resources\Head\Model.obj");
            screen.Render(model, shader, zBuffer);
        }

        private static void RenderShadowShader(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var shadowBuffer = new float[ScreenWidth, ScreenHeight];

            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            var light = new Vector3(1, 1, 1);

            light = (projection * modelView).Multiply(light).Normalize();

            var shadowProjection = Matrix4x4.Identity;
            var shadowModelView = CreateLookAt(light, cameraTarget, cameraUp);

            var model = ModelUtils.LoadModel(@"Resources\Diablo3\Model.obj");
            var texture = TextureUtils.LoadTexture(@"Resources\Diablo3\Diffuse.png");
            var normalMap = TextureUtils.LoadTexture(@"Resources\Diablo3\Normal.png");
            var specularMap = TextureUtils.LoadTexture(@"Resources\Diablo3\Specular.png").Flip(FlipType.Vertical);

            IShader shader = new ZBufferShader
            {
                Transform = viewPort * shadowProjection * shadowModelView
            };

            screen.Render(model, shader, shadowBuffer);

            shader = new ShadowShader
            {
                Transform = viewPort * projection * modelView,
                MIT = (projection * modelView).Invert().Transpose(),
                ShadowTransform = viewPort * shadowProjection * shadowModelView *
                                  (viewPort * projection * modelView).Invert(),
                Light = light,

                Texture = texture,
                NormalMap = normalMap,
                SpecularMap = specularMap,

                ShadowBuffer = shadowBuffer
            };

            screen.Fill(Rgba32.Black);
            screen.Render(model, shader, zBuffer);
        }

        private static void GenerateOccolusionMap()
        {
            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);

            var model = ModelUtils.LoadModel(@"Resources\Diablo3\Model.obj");
            var occlusionMap = model.GenerateOccolusionMap(viewPort, ScreenWidth, ScreenHeight, 1000);

            using (var output = File.OpenWrite("Occolusion.png"))
            {
                occlusionMap.Flip(FlipType.Vertical).Save(output, new PngEncoder());
            }
        }

        private static void RenderAmbientOccolusion(this Image<Rgba32> screen, float[,] zBuffer)
        {
            var cameraPosition = new Vector3(1, 1, 3);
            var cameraTarget = new Vector3(0, 0, 0);
            var cameraUp = new Vector3(0, 1, 0);

            var viewPort = CreateViewPort(ScreenWidth / 8, ScreenHeight / 8, ScreenWidth * 3 / 4, ScreenHeight * 3 / 4);
            var projection = CreateProjection(-1 / (cameraPosition - cameraTarget).Length());
            var modelView = CreateLookAt(cameraPosition, cameraTarget, cameraUp);

            IShader shader = new ZBufferShader
            {
                Transform = viewPort * projection * modelView
            };

            var model = ModelUtils.LoadModel(@"Resources\Diablo3\Model.obj");
            screen.Render(model, shader, zBuffer);

            screen.RenderAmbientOccolusion(zBuffer, ScreenWidth, ScreenHeight);
        }
    }
}