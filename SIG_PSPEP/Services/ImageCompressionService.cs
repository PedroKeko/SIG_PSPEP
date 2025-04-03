using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace SIG_PSPEP.Services
{
    public class ImageCompressionService
    {
        public async Task<byte[]> CompressImageAsync(Stream imageStream, string fileExtension)
        {
            byte[] compressedImageBytes;

            using (var image = Image.Load(imageStream)) // Carrega a imagem a partir do stream
            {
                // Reduz a resolução da imagem para um tamanho mais adequado
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(1024, 1024) // Ajuste a resolução conforme necessário
                }));

                var qualidade = 90; // Qualidade inicial para compressão

                using (var ms = new MemoryStream())
                {
                    IImageEncoder encoder;

                    // Ajusta o encoder dependendo da extensão
                    if (fileExtension == ".png")
                    {
                        encoder = new PngEncoder();
                    }
                    else
                    {
                        encoder = new JpegEncoder { Quality = qualidade };
                    }

                    // Ajusta a qualidade da imagem até que ela atenda ao limite de 60 KB
                    do
                    {
                        ms.SetLength(0); // Limpa o conteúdo do MemoryStream
                        image.Save(ms, encoder); // Salva a imagem comprimida no MemoryStream

                        // Verifica o tamanho da imagem após a compressão
                        if (ms.Length <= 60 * 1024) break; // Se for menor que 60 KB, saímos do loop

                        // Reduz a qualidade da imagem para continuar a compressão
                        qualidade -= 5; // Diminui a qualidade para tentar comprimir mais

                        // Garante que a qualidade não seja inferior a 1 e não ultrapasse 100
                        if (qualidade < 1) qualidade = 1;
                        if (qualidade > 100) qualidade = 100;

                        // Recria o encoder com a nova qualidade
                        if (fileExtension == ".png")
                        {
                            encoder = new PngEncoder();
                        }
                        else
                        {
                            encoder = new JpegEncoder { Quality = qualidade };
                        }
                    }
                    while (ms.Length > 60 * 1024); // Continue comprimindo até atingir o tamanho desejado

                    compressedImageBytes = ms.ToArray(); // Obtém o array de bytes da imagem comprimida
                }
            }

            return compressedImageBytes;
        }




        public async Task<byte[]> GetDefaultImageAsync()
        {
            // Caminho da imagem padrão (estática) no wwwroot
            var caminhoImagemPadrao = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "users", "user-police.png");

            if (!File.Exists(caminhoImagemPadrao))
            {
                throw new FileNotFoundException("Imagem padrão não encontrada no caminho especificado.");
            }

            return await File.ReadAllBytesAsync(caminhoImagemPadrao);
        }

    }
}