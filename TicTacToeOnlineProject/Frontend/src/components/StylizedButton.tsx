interface StylizedButtonProps {
    functionCallback: () => void
    isVisible: boolean
    text: string
    color: string
    largeBorder: boolean
    hasHover: boolean
}

export default function StylizedButton({ functionCallback, isVisible, text, color, largeBorder, hasHover}: StylizedButtonProps) {
    return (
        <button
            onClick={functionCallback}
            className={`w-full rounded-lg py-2.5 text-sm font-bold transition-all
                ${
                    isVisible
                    ? `border-[#1E293B] ${color} text-[#1E293B] shadow-[2px_2px_0px_0px_#1E293B]`
                    : "text-slate-600 hover:text-[#1E293B]"
                }
                ${
                    hasHover
                    ? "transition-all hover:-translate-y-0.5 active:translate-x-0.5 active:translate-y-0.5"
                    : ""
                }
                ${
                    largeBorder
                    ? "border-[2.5px]"
                    : "border-2"
                }
            `}
            >
            {text}
        </button>
    )
}