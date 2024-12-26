using FluentAssertions;

namespace Bowling;

public class BowlingTest
{
    private BowlingGame _bowlingGame;

    [SetUp]
    public void Setup()
    {
        _bowlingGame = new BowlingGame();
    }

    [Test]
    public void frame_one_single_ball()
    {
        _bowlingGame.Throw(2);

        ResultShouldBe([
            new FrameInfoOut
            {
                First = "2",
                Second = null,
                Score = null
            }
        ]);
    }

    [Test]
    public void frame_one_remaining_pins()
    {
        _bowlingGame.Throw(2);
        _bowlingGame.Throw(7);

        ResultShouldBe([
            new FrameInfoOut
            {
                First = "2",
                Second = "7",
                Score = 9
            }
        ]);
    }

    [Test]
    public void frame_one_spare()
    {
        _bowlingGame.Throw(3);
        _bowlingGame.Throw(7);

        ResultShouldBe([
            new FrameInfoOut
            {
                First = "3",
                Second = "/",
                Score = null
            }
        ]);
    }

    [Test]
    public void frame_one_strike()
    {
        _bowlingGame.Throw(10);

        ResultShouldBe([
            new FrameInfoOut
            {
                First = "x",
                Second = null,
                Score = null
            }
        ]);
    }

    [Test]
    public void frame_second_first_pin_and_frame_one_spare()
    {
        _bowlingGame.Throw(3);
        _bowlingGame.Throw(7);
        _bowlingGame.Throw(5);

        ResultShouldBe([
            new FrameInfoOut
            {
                First = "3",
                Second = "/",
                Score = 15
            },
            new FrameInfoOut
            {
                First = "5",
                Second = null,
                Score = null
            }
        ]);
    }


    private void ResultShouldBe(List<FrameInfoOut> expectation)
    {
        var frameInfos = _bowlingGame.GetResult();
        frameInfos.Should().BeEquivalentTo(expectation);
    }
}

public class BowlingGame
{
    private readonly List<FrameInfo> _frameInfos = new();

    public void Throw(int pins)
    {
        if (!_frameInfos.Any())
        {
            _frameInfos.Add(new FrameInfo
            {
                Index = 1,
                First = pins
            });
        }
        else
        {
            var frameInfo = _frameInfos.Last();
            if (frameInfo.IsCompleted)
            {
                // frameInfo.UpdateExtraScore()
                _frameInfos.Add(new FrameInfo
                {
                    Index = 1,
                    First = pins
                });
            }
            else
            {
                frameInfo.Second = pins;
                frameInfo.Score = frameInfo.First + frameInfo.Second;
            }
        }
    }

    public List<FrameInfoOut> GetResult()
    {
        return _frameInfos.Select(frame => frame.ToResult()).ToList();
    }
}

public class FrameInfoOut
{
    public string First { get; set; }
    public string? Second { get; set; }
    public int? Score { get; set; }
}

public class FrameInfo
{
    public int Index { get; set; }
    public int First { get; set; }
    public int? Second { get; set; }
    public int? Score { get; set; }
    public bool IsCompleted => Second is not null;

    public FrameInfoOut ToResult()
    {
        var isStrike = First == 10;
        if (isStrike)
            return new FrameInfoOut
            {
                First = "x"
            };
        var isSpare = First + Second == 10;

        return new FrameInfoOut
        {
            First = First.ToString(),
            Second = isSpare ? "/" : Second?.ToString(),
            Score = isSpare ? null : Score
        };
    }
}