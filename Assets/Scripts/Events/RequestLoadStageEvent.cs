namespace PuzzleRoom.Event
{
    public struct RequestLoadStageEvent
    {
        public readonly int StageId;
        public RequestLoadStageEvent(int stageId) => StageId = stageId;
    }

    public struct RequestReLoadStageEvent
    {
        
    }
}