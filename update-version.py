from pathlib import Path
import re
import argparse

def read_value(file_path: Path, property: str):
    with open(file_path, 'r') as file:
        content = file.read()
        match = re.search(property + r'\("([^"]+)"\)', content)
        if match:
            return match.group(1)

def set_value(new_version: str, file_path: Path, property: str):
    with open(file_path, 'r') as file:
        content = file.read()
        updated_content = re.sub(property + r'\("[^"]+"\)', f'{property}("{new_version}")', content)
        with open(file_path, 'w') as updated_file:
            updated_file.write(updated_content)

    print(f"{file_path} - {property} = {new_version}")


def short_semver(semver: str):
    match = re.search(r'\d+\.\d+\.\d+', semver)
    if match:
        version = match.group()
        return version
    else:
        raise ValueError(f"Invalid semver format: {semver}")

def main():
    parser = argparse.ArgumentParser(description='Update version in AssemblyInfo.cs file.')
    parser.add_argument('new_version', type=str, help='New version in semver format (e.g., 1.2.3)')

    args = parser.parse_args()

    assembly_info_path = "BetterRocketDesigns/Properties/AssemblyInfo.cs"

    current_version = read_value(assembly_info_path, 'AssemblyVersion')

    converted_version = short_semver(args.new_version)

    print(f"Current version: {current_version}")
    set_value(f"{converted_version}.0", assembly_info_path, 'AssemblyVersion')
    set_value(f"{converted_version}.0", assembly_info_path, 'AssemblyFileVersion')
    print(f"Complete version update to {args.new_version} ({converted_version}.0)")

if __name__ == "__main__":
    main()