namespace RestauranteEstoque.Tests.Integration;

public sealed class DockerFactAttribute : FactAttribute
{
    private const string MotivoPulo = "Docker indisponível para os testes de integração com SQL Server.";

    public DockerFactAttribute()
    {
        if (!DockerDisponivel())
            Skip = MotivoPulo;
    }

    private static bool DockerDisponivel()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOCKER_HOST")))
            return true;

        if (OperatingSystem.IsWindows())
            return File.Exists(@"\\.\pipe\docker_engine");

        return File.Exists("/var/run/docker.sock");
    }
}
