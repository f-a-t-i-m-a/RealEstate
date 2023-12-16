
interface String {
    startsWith(prefix: string): boolean;
    endsWith(suffix: string): boolean;
    trimEnd(): string;
    trimStart(): string;
}

interface StringConstructor {
    format(format: string, ...args: any[]): string;
}
