import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useSignalR } from "../context/SignalRContext";
import StylizedButton from "../components/StylizedButton";
import LoadingContainer from "../components/LoadingContainer";
import type { AssignMarkResponse, BoardDto, JoinGameResponse } from "./Home";
import ConnectionFailed from "../components/ConnectionFailed";

export type MoveReturnStatus = 
  | "MoveSuccess"
  | "BoardNotFound"
  | "BoardNullException"
  | "IndexOutOfRange"
  | "NotCurrentTurn"
  | "CellNotEmpty"
  | "GameFinished";

export type RematchReturnStatus = 
  | "BoardNotFound"
  | "BoardNullException"
  | "Waiting"
  | "RematchAccepted"
  | "RematchDenied";

export interface MoveResponse {
  success: boolean;
  status: MoveReturnStatus | null;
  updatedBoardDto: BoardDto | null;
  isGameOver: boolean;
  winnerMark: string | null;
}

export interface RematchResponse {
  success: boolean;
  status: RematchReturnStatus;
  boardDto: BoardDto | null;
}

interface Particle {
  id: number;
  left: number;
  color: string;
  size: number;
  delay: number;
  duration: number;
  rotation: number;
}

export default function GameBoard() {
  const navigate = useNavigate();
  const { connection, isConnected, isInitialLoading, reconnect } = useSignalR();

  const { roomCode } = useParams<"roomCode">();
  const [boardDto, setBoardDto] = useState<BoardDto | null>(null);
  const [playerMark, setPlayerMark] = useState<string | null>(null);
  const [opponentMark, setOpponentMark] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentTurn, setCurrentTurn] = useState<string>("");
  const [winner, setWinner] = useState<string | null>(null);
  const [isGameOver, setIsGameOver] = useState(false);
  const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(true);
  const [isRematchRequested, setIsRematchRequested] = useState(false);
  const [hasIncomingRematch, setHasIncomingRematch] = useState(false);
  const [rematchDeclined, setRematchDeclined] = useState(false);
  const [opponentLeft, setOpponentLeft] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isRetrying, setIsRetrying] = useState(false);
  const [copied, setCopied] = useState(false);
  const [particles, setParticles] = useState<Particle[]>([]);
  const hasJoinedRef = useRef(false);

  useEffect(() => {
    if (!isGameOver) {
      setParticles([]);
      return;
    }

    const isWinner = winner === playerMark;
    const count = isWinner ? 45 : 30;
    const colors = isWinner
      ? ["#FACC15", "#3B82F6", "#10B981", "#EF4444", "#EC4899", "#8B5CF6"]
      : ["#64748B", "#94A3B8", "#475569", "#CBD5E1"];

    const generated: Particle[] = Array.from({ length: count }, (_, i) => ({
      id: i,
      left: Math.random() * 100,
      color: colors[Math.floor(Math.random() * colors.length)],
      size: isWinner ? Math.floor(Math.random() * 8) + 10 : Math.floor(Math.random() * 4) + 6,
      delay: Math.random() * 2,
      duration: isWinner ? Math.random() * 2 + 2.5 : Math.random() * 1.5 + 1.8,
      rotation: Math.floor(Math.random() * 360),
    }));

    setParticles(generated);

    const timer = setTimeout(() => {
      setParticles([]);
    }, 3000);

    return () => clearTimeout(timer);
  }, [isGameOver, winner, playerMark]);

  useEffect(() => {
    if (!roomCode) return;

    async function initGame() {
      setLoading(true);

      if (!roomCode || !connection || hasJoinedRef.current) return;
      hasJoinedRef.current = true;

      try {
        const joinGameResponse = await connection?.invoke<JoinGameResponse>(
          "JoinGameRoom", roomCode
        );
        if (joinGameResponse == null || !joinGameResponse.success) {
          setErrorMessage("Failed to join game");
          console.error("Failed to join game:", joinGameResponse?.status);
          return;
        }

        const getMarkResponse = await connection?.invoke<AssignMarkResponse>(
          "GetMark", roomCode
        );
        if (getMarkResponse == null || !getMarkResponse.success) {
          setErrorMessage("Failed to get assigned mark");
          console.error("Failed to get assigned mark:", getMarkResponse?.status);
          return;
        }

        const assignedMark = getMarkResponse.mark;

        setBoardDto(joinGameResponse.boardDto);
        setCurrentTurn(joinGameResponse.boardDto?.currentTurn!);
        setPlayerMark(assignedMark);
        setOpponentMark(assignedMark?.trim().toUpperCase() === 'X' ? 'O' : 'X');

        if (joinGameResponse.boardDto && joinGameResponse.boardDto.playersPresent >= 2) {
          setIsWaitingForOpponent(false);
        }
      } catch (err) {
        console.error("Error in GameBoard.useEffect.initGame:", err);
      } finally {
        setLoading(false);
      }
    }

    const handleNotifyGroupOnPlayerJoin = () => {
      setIsWaitingForOpponent(false);
      setOpponentLeft(false);
    };

    const handleNotifyGroupOnPlayerLeave = () => {
      setOpponentLeft(true);
    };

    const handleGameUpdated = (board: BoardDto) => {
      setBoardDto(board);
      setCurrentTurn(board.currentTurn);
    };

    const handleGameOver = (board: BoardDto, winnerMark: string) => {
      setBoardDto(board);
      setWinner(winnerMark === "D" ? "Draw" : winnerMark);
      setIsGameOver(true);
    };

    const handleRematchRequest = (response: RematchResponse) => {
      if (response.status === "Waiting") {
        setHasIncomingRematch(true);
      } else if (response.status === "RematchAccepted" && response.boardDto) {
        const updatedBoard = response.boardDto;

        if (connection?.connectionId && updatedBoard.playerMarks) {
          const myNewMark = updatedBoard.playerMarks[connection.connectionId];
          if (myNewMark) {
            setPlayerMark(myNewMark);
            setOpponentMark(myNewMark.toUpperCase() === "X" ? "O" : "X");
          }
        }

        setBoardDto(updatedBoard);
        setCurrentTurn(updatedBoard.currentTurn);
        setIsGameOver(false);
        setWinner(null);
        setIsRematchRequested(false);
        setHasIncomingRematch(false);
        setRematchDeclined(false);
        setErrorMessage(null);
      } else if (response.status === "RematchDenied") {
        setIsRematchRequested(false);
        setHasIncomingRematch(false);
        setRematchDeclined(true);
      }
    };

    connection?.on("NotifyGroupOnPlayerJoin", handleNotifyGroupOnPlayerJoin);
    connection?.on("NotifyGroupOnPlayerLeave", handleNotifyGroupOnPlayerLeave);
    connection?.on("GameUpdated", handleGameUpdated);
    connection?.on("GameOver", handleGameOver);
    connection?.on("RematchRequest", handleRematchRequest);

    initGame();

    return () => {
      connection?.off("NotifyGroupOnPlayerJoin", handleNotifyGroupOnPlayerJoin);
      connection?.off("NotifyGroupOnPlayerLeave", handleNotifyGroupOnPlayerLeave);
      connection?.off("GameUpdated", handleGameUpdated);
      connection?.off("GameOver", handleGameOver);
      connection?.off("RematchRequest", handleRematchRequest);
    };
  }, [roomCode, connection]);

  const isMyTurn = () => currentTurn === playerMark;

  const handleLeave = async () => {
    if (roomCode) {
      try {
        await connection?.invoke("LeaveGame", roomCode);
      } catch (err) {
        console.error("Error leaving game:", err);
      }
    }
    navigate("/");
  };

  const handleCellClick = async (index: number) => {
    try {
      const response = await connection?.invoke<MoveResponse>(
        "MakeMove",
        roomCode,
        index,
        playerMark
      );

      if (response == null || !response?.success) {
        setErrorMessage("Failed to execute move. Please try again");
        console.error("Failed to execute move:", response?.status);
        return;
      }

      if (response.status === "CellNotEmpty") {
        setErrorMessage("Cell is not empty. Please try again");
        return;
      }

      if (response.status === "NotCurrentTurn") {
        setErrorMessage("Not your turn. Please wait for opponent");
        return;
      }
    } catch (err) {
      console.error("Error in GameBoard.handleCellClick:", err);
    }
  };

  const handleRematch = async () => {
    if (rematchDeclined || isRematchRequested) return;

    try {
      setRematchDeclined(false);
      setIsRematchRequested(true);
      await connection?.invoke("Rematch", roomCode, true);
    } catch (err) {
      console.error("Error sending rematch request:", err);
      setIsRematchRequested(false);
    }
  };

  const handleAcceptRematch = async () => {
    try {
      setHasIncomingRematch(false);

      const response = await connection?.invoke<RematchResponse>("Rematch", roomCode, true);
      
      if (response && response.status === "RematchAccepted" && response.boardDto) {
        setBoardDto(response.boardDto);
        setCurrentTurn(response.boardDto.currentTurn);
        setIsGameOver(false);
        setWinner(null);
        setIsRematchRequested(false);
        setRematchDeclined(false);
        setErrorMessage(null);
      }
    } catch (err) {
      console.error("Error accepting rematch:", err);
    }
  };

  const handleDeclineRematch = async () => {
    setHasIncomingRematch(false);
    if (roomCode) {
      try {
        await connection?.invoke("Rematch", roomCode, false);
        await connection?.invoke("LeaveGame", roomCode);
      } catch (err) {
        console.error("Error declining rematch and leaving:", err);
      }
    }
    navigate("/");
  };

  const handleReconnect = async () => {
    setIsRetrying(true);
    try {
      await reconnect();
    } catch (err) {
      console.error("Manual reconnect failed:", err);
    } finally {
      setIsRetrying(false);
    }
  };

  const handleCopyRoomCode = async () => {
    if (!roomCode) return;

    try {
      if (navigator?.clipboard?.writeText) {
        await navigator.clipboard.writeText(roomCode);
      } else {
        const textArea = document.createElement("textarea");
        textArea.value = roomCode;
        textArea.style.position = "fixed";
        textArea.style.opacity = "0";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        document.execCommand("copy");
        document.body.removeChild(textArea);
      }

      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error("Failed to copy code:", err);
    }
  };

  const getBackgroundTint = () => {
    if (opponentLeft) return "bg-transparent";

    if (isGameOver) {
      if (winner === playerMark) {
        return "bg-emerald-500/8";
      }
      return "bg-rose-500/8";
    }

    return isMyTurn() ? "bg-emerald-500/8" : "bg-rose-500/8";
  };

  if (!isConnected) {
    return (
      <div className="w-full max-w-sm mx-auto flex flex-col items-center">
        <ConnectionFailed text="Failed to connect to the server" functionCallback={handleReconnect} />
      </div>
    );
  }

  if (isInitialLoading || isRetrying) {
    return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
  }

  if (loading && !errorMessage) {
    return <LoadingContainer text="Joining room" styleSelected="bg-[#FACC15]" />;
  }

  const isWinner = winner === playerMark;

  return (
    <>
      {/* Dynamic Keyframe Animations for Neo-Brutalist Confetti & Rain */}
      <style>{`
        @keyframes confettiDrop {
          0% {
            transform: translateY(-10vh) rotate(0deg) scale(0.7);
            opacity: 1;
          }
          75% {
            opacity: 1;
          }
          100% {
            transform: translateY(105vh) rotate(720deg) scale(1);
            opacity: 0;
          }
        }
        @keyframes sadRainDrop {
          0% {
            transform: translateY(-10vh) scaleY(1);
            opacity: 0.7;
          }
          85% {
            opacity: 0.7;
          }
          100% {
            transform: translateY(105vh) scaleY(1.3);
            opacity: 0;
          }
        }
      `}</style>

      {/* Game Over Celebration / Loss Particles Overlay (3s Timer) */}
      {isGameOver && particles.length > 0 && (
        <div className="pointer-events-none fixed inset-0 z-30 overflow-hidden">
          {particles.map((p) =>
            isWinner ? (
              <div
                key={p.id}
                style={{
                  left: `${p.left}%`,
                  width: `${p.size}px`,
                  height: `${p.size}px`,
                  backgroundColor: p.color,
                  animation: `confettiDrop ${p.duration}s cubic-bezier(0.25, 0.46, 0.45, 0.94) infinite`,
                  animationDelay: `${p.delay}s`,
                  transform: `rotate(${p.rotation}deg)`,
                }}
                className="absolute -top-6 rounded-xs border-[1.5px] border-[#1E293B] shadow-[1.5px_1.5px_0px_0px_#1E293B]"
              />
            ) : (
              <div
                key={p.id}
                style={{
                  left: `${p.left}%`,
                  width: `${p.size}px`,
                  height: `${p.size * 2}px`,
                  backgroundColor: p.color,
                  animation: `sadRainDrop ${p.duration}s linear infinite`,
                  animationDelay: `${p.delay}s`,
                }}
                className="absolute -top-8 rounded-full border-[1.5px] border-[#1E293B]"
              />
            )
          )}
        </div>
      )}

      {/* Background Tint Overlay */}
      <div
        className={`pointer-events-none fixed inset-0 z-0 transition-colors duration-500 ease-in-out ${getBackgroundTint()}`}
      />

      <div className="relative z-10 flex w-full flex-col items-center gap-4">
        {/* Rematch Overlay Modal */}
        {hasIncomingRematch && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-xs px-4">
            <div className="flex w-full max-w-xs flex-col items-center gap-5 rounded-3xl border-[3.5px] border-[#1E293B] bg-white p-6 shadow-[8px_8px_0px_0px_#1E293B]">
              <div className="flex flex-col items-center gap-1.5 text-center">
                <span className="text-2xl">⚔️</span>
                <h3 className="text-lg font-black text-[#1E293B]">Rematch Request</h3>
                <p className="text-xs font-bold text-slate-500">
                  Opponent wants to play another round!
                </p>
              </div>

              <div className="flex w-full flex-col gap-2.5">
                <StylizedButton
                  isSelected={true}
                  text="Accept"
                  color="bg-[#10B981]"
                  borderSize="medium"
                  shadowSize="medium"
                  textSelectedColor="text-white"
                  textStyle="text-sm font-black tracking-wide"
                  functionCallback={handleAcceptRematch}
                />
                <StylizedButton
                  isSelected={true}
                  text="Decline"
                  color="bg-[#EF4444]"
                  borderSize="medium"
                  shadowSize="medium"
                  textSelectedColor="text-white"
                  textStyle="text-sm font-black tracking-wide"
                  functionCallback={handleDeclineRematch}
                />
              </div>
            </div>
          </div>
        )}

        {/* Top-Right Leave Button */}
        <div className="fixed top-6 right-6 z-40">
          <StylizedButton
            isSelected={true}
            text="Leave"
            color="bg-[#EF4444]"
            padding="px-4 py-2"
            borderSize="small"
            shadowSize="small"
            customStyles="w-auto text-white font-bold"
            functionCallback={handleLeave}
          />
        </div>

        {/* Centered Room Code Header */}
        <div className="flex w-full flex-col items-center justify-center px-1">
          <span className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Room
          </span>

          <div className="flex items-center gap-1.5">
            <span className="text-xl font-bold uppercase tracking-wider text-[#1E293B]">
              {roomCode}
            </span>

            <button
              type="button"
              onClick={handleCopyRoomCode}
              aria-label="Copy Room Code"
              title={copied ? "Copied!" : "Copy code"}
              className="flex items-center justify-center p-1 text-slate-400 transition-colors hover:text-[#1E293B] active:scale-90"
            >
              {copied ? (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                  className="h-4 w-4 text-emerald-600"
                >
                  <path
                    fillRule="evenodd"
                    d="M16.704 4.153a.75.75 0 0 1 .143 1.052l-8 10.5a.75.75 0 0 1-1.127.075l-4.5-4.5a.75.75 0 0 1 1.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 0 1 1.05-.143Z"
                    clipRule="evenodd"
                  />
                </svg>
              ) : (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                  className="h-4 w-4"
                >
                  <path d="M7 3.5A1.5 1.5 0 0 1 8.5 2h3.879a1.5 1.5 0 0 1 1.06.44l3.122 3.12A1.5 1.5 0 0 1 17 6.622V12.5a1.5 1.5 0 0 1-1.5 1.5h-1v-3.379a3 3 0 0 0-.879-2.121L10.5 5.379A3 3 0 0 0 8.379 4.5H7v-1Z" />
                  <path d="M4.5 6A1.5 1.5 0 0 0 3 7.5v9A1.5 1.5 0 0 0 4.5 18h7a1.5 1.5 0 0 0 1.5-1.5v-5.879a1.5 1.5 0 0 0-.44-1.06L9.44 6.439A1.5 1.5 0 0 0 8.378 6H4.5Z" />
                </svg>
              )}
            </button>
          </div>
        </div>

        {/* Players Header */}
        <div className="grid w-full grid-cols-3 items-center px-2">
          {/* You */}
          <div className="flex flex-col items-start gap-1">
            <div className="flex items-center gap-2">
              <div className="flex h-9 w-9 items-center justify-center rounded-full border-[2.5px] border-[#1E293B] bg-[#3B82F6] font-black text-white shadow-[2px_2px_0px_0px_#1E293B]">
                {playerMark || "?"}
              </div>
              <span className="text-base font-black text-[#1E293B]">You</span>
            </div>
          </div>

          {/* VS */}
          <div className="text-center">
            <span className="text-sm font-extrabold tracking-wider text-slate-400">VS</span>
          </div>

          {/* Opponent */}
          <div className="flex flex-col items-end gap-1">
            <div className="flex items-center gap-2">
              <span className="text-base font-black text-[#1E293B]">Opponent</span>
              <div className="flex h-9 w-9 items-center justify-center rounded-full border-[2.5px] border-[#1E293B] bg-[#EF4444] font-black text-white shadow-[2px_2px_0px_0px_#1E293B]">
                {opponentMark || "?"}
              </div>
            </div>
          </div>
        </div>

        {/* Turn or Game Over Status Pill */}
        <div className="my-1">
          <div
            className={`rounded-2xl border-[2.5px] border-[#1E293B] px-6 py-2 text-sm font-black text-white shadow-[3px_3px_0px_0px_#1E293B] transition-all ${
              isGameOver
                ? winner === playerMark
                  ? "bg-[#3B82F6]"
                  : winner === "Draw"
                    ? "bg-slate-500"
                    : "bg-[#EF4444]"
                : isMyTurn()
                  ? "bg-[#3B82F6]"
                  : "bg-slate-400"
            }`}
          >
            {isGameOver 
              ? winner === playerMark 
                ? "You won!" 
                : winner === "Draw" 
                  ? "It's a draw!" 
                  : "You lost!"
              : isMyTurn()
                ? "Your turn" 
                : "Opponent's turn"}
          </div>
        </div>

        {/* 3x3 Grid Container */}
        <div className="aspect-square w-full max-w-95 overflow-hidden rounded-4xl border-[3.5px] border-[#1E293B] bg-white shadow-[8px_8px_0px_0px_#1E293B]">
          <div className="grid h-full w-full grid-cols-3 grid-rows-3">
            {boardDto?.grid.map((cellValue, index) => {
              const isRightBorder = index % 3 !== 2;
              const isBottomBorder = index < 6;
              const isMyMark = cellValue === playerMark;

              return (
                <button
                  key={index}
                  type="button"
                  disabled={!isMyTurn() || cellValue !== "" || isGameOver}
                  onClick={() => handleCellClick(index)}
                  className={`flex items-center justify-center transition-colors ${
                    isRightBorder ? "border-r-[3.5px] border-[#1E293B]" : ""
                  } ${isBottomBorder ? "border-b-[3.5px] border-[#1E293B]" : ""} ${
                    !cellValue && isMyTurn() && !isGameOver ? "hover:bg-[#FFFBEA]" : ""
                  }`}
                >
                  {cellValue && (
                    <span
                      className={`text-4xl font-black sm:text-5xl ${
                        isMyMark ? "text-[#3B82F6]" : "text-[#EF4444]"
                      }`}
                    >
                      {cellValue}
                    </span>
                  )}
                </button>
              );
            })}
          </div>
        </div>

        {/* Bottom Action Area */}
        <div className="w-full max-w-95 mt-5 mb-10">
          {isGameOver ? (
            <StylizedButton
              isSelected={true}
              text={
                rematchDeclined
                  ? "Opponent declined rematch"
                  : isRematchRequested
                    ? "Waiting for Opponent..."
                    : "Rematch"
              }
              color={rematchDeclined ? "bg-slate-200" : "bg-[#FACC15]"}
              borderSize="large"
              shadowSize="large"
              padding="py-3.5 px-4"
              textStyle={`text-base font-black tracking-wide ${rematchDeclined ? "text-slate-500" : "text-[#1E293B]"}`}
              customStyles="rounded-2xl"
              functionCallback={rematchDeclined ? handleLeave : handleRematch}
              hasHover={!isRematchRequested}
            />
          ) : (
            <StylizedButton 
              text={
                errorMessage
                  ? errorMessage
                  : opponentLeft
                    ? "Opponent has left the game"
                    : isWaitingForOpponent
                      ? "Waiting for opponent to join..."
                      : isMyTurn()
                        ? "Tap a square to play"
                        : "Waiting for opponent move..."
              }
              color="bg-[#FACC15]"
              hasHover={false}
              isSelected={true}
              borderSize="large"
              shadowSize="large"
              padding="py-3.5 px-4"
              textStyle="text-base font-black tracking-wide"
            />
          )}
        </div>
      </div>
    </>
  );
}