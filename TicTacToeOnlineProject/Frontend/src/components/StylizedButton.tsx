export type ShadowSize = "none" | "small" | "medium" | "large";
export type BorderSize = "none" | "small" | "medium" | "large";

interface StylizedButtonProps {
  functionCallback?: () => void;
  isSelected?: boolean;
  text: string;
  textStyle?: string
  textSelectedColor?: string
  textUnselectedColor?: string
  padding?: string;
  color?: string;
  shadowSize?: ShadowSize;
  borderSize?: BorderSize;
  hasHover?: boolean;
  customStyles?: string;
}

const shadowStyles: Record<ShadowSize, string> = {
  none: "",
  small: "shadow-[2px_2px_0px_0px_#1E293B] active:shadow-[1px_1px_0px_0px_#1E293B]",
  medium: "shadow-[4px_4px_0px_0px_#1E293B]",
  large: "shadow-[8px_8px_0px_0px_#1E293B] active:shadow-[2px_2px_0px_0px_#1E293B]",
};

const borderStyles: Record<BorderSize, string> = {
  none: "border-0",
  small: "border-[2px] border-[#1E293B]",
  medium: "border-[2.5px] border-[#1E293B] rounded-xl",
  large: "border-[3px] border-[#1E293B]",
};

export default function StylizedButton({
  functionCallback,
  isSelected = false,
  text,
  textStyle = "text-sm font-bold",
  textSelectedColor = "text-[#1E293B]",
  textUnselectedColor = "text-slate-300",
  padding = "py-2.5 px-4",
  color = "bg-[#FACC15]",
  shadowSize = "small",
  borderSize = "small",
  hasHover = true,
  customStyles
}: StylizedButtonProps) {
  const selectedStyles = isSelected
    ? `${color} ${textSelectedColor} ${borderStyles[borderSize]} ${shadowStyles[shadowSize]}`
    : `border-0 shadow-none ${textUnselectedColor} hover:text-[#1E293B] bg-transparent`;

  const hoverStyles = hasHover
    ? "transition-all hover:-translate-y-0.5 active:translate-x-0.5 active:translate-y-0.5"
    : "";

  return (
    <button
      type="button"
      onClick={functionCallback}
      className={`w-full rounded-lg ${textStyle} ${padding} ${selectedStyles} ${hoverStyles} ${customStyles}`}
    >
      {text}
    </button>
  );
}