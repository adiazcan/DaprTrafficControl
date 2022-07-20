var httpClient = new HttpClient();
int lanes = 3;

CameraSimulation[] cameras = new CameraSimulation[lanes];
for (var i = 0; i < lanes; i++)
{
    int camNumber = i + 1;
    var trafficControlService = new HttpTrafficControlService(httpClient);
    cameras[i] = new CameraSimulation(camNumber, trafficControlService);
}
Parallel.ForEach(cameras, cam => cam.Start());

Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();