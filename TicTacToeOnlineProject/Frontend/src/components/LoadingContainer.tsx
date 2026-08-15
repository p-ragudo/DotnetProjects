import { useEffect, useState } from "react";
import StylizedButton from "./StylizedButton";

interface LoadingContainerProps {
  text: string;
  styleSelected: string;
}

export default function LoadingContainer({ text, styleSelected }: LoadingContainerProps) {
  const [dotCount, setDotCount] = useState(0);

  // Clean any existing trailing dots
  const baseText = text.replace(/\.+$/, "");

  useEffect(() => {
    const interval = setInterval(() => {
      setDotCount((prev) => (prev + 1) % 4); // cycles: 0 -> 1 -> 2 -> 3 -> 0
    }, 400);

    return () => clearInterval(interval);
  }, []);

  // Directly append the dots without whitespace padding
  const animatedText = `${baseText}${".".repeat(dotCount)}`;

  return (
    <StylizedButton 
      isSelected={true}
      text={animatedText}
      padding="py-3"
      color={styleSelected}
      shadowSize="large"
      borderSize="large"
      hasHover={false}
    />
  );
}