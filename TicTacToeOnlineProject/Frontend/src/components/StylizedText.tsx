export type StylizedTextSize = "xs" | "small" | "medium" | "large" | "xl";

// Preserves IDE autocomplete while allowing any custom class string
export type StylizedTextColorPreset = "black" | "gray" | "semi-black";
export type StylizedTextColor = StylizedTextColorPreset | (string & {});

interface StylizedTextProps {
  text: string;
  size?: StylizedTextSize;
  color?: StylizedTextColor;
  fontThickness?: string;
  customStyles?: string
}

const textStyles: Record<StylizedTextSize, string> = {
  xs: "text-xs",
  small: "text-sm",
  medium: "text-base",
  large: "text-lg",
  xl: "text-xl",
};

const presetColors: Record<StylizedTextColorPreset, string> = {
  black: "text-[#1E293B]",
  gray: "text-slate-500",
  "semi-black": "text-slate-700"
};

export default function StylizedText({
  text,
  size = "medium",
  color = "black",
  fontThickness = "font-semibold",
  customStyles
}: StylizedTextProps) {
  // If 'color' exists in presets, use mapped class; otherwise, use 'color' directly as the class
  const resolvedColor = color in presetColors 
    ? presetColors[color as StylizedTextColorPreset] 
    : color;

  return (
    <p className={`${resolvedColor} ${fontThickness} ${textStyles[size]} ${customStyles}`}>
      {text}
    </p>
  );
}