export function bytesToHuman(bytes: number = 0, precision: number = 2): string {
    const units = [
        'bytes',
        'KB',
        'MB',
        'GB',
        'TB',
        'PB'
    ]
    if (isNaN(parseFloat(String(bytes))) || !isFinite(bytes)) return '?';

    let unit = 0;

    while (bytes >= 1024) {
        bytes /= 1024;
        unit++;
    }

    return bytes.toFixed(+precision) + ' ' + units[unit];
}

const nameof = <T>(name: keyof T) => name;
