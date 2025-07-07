namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class TrainingEntryPointData
    {
        public readonly TrainingFactory TrainingFactory;
        public readonly TrainingConfigsProviderService TrainingConfigsProviderService;

        public TrainingEntryPointData(TrainingFactory trainingFactory, TrainingConfigsProviderService trainingConfigsProviderService)
        {
            TrainingFactory = trainingFactory;
            TrainingConfigsProviderService = trainingConfigsProviderService;
        }
    }
}