from pathlib import Path
import re
import argparse
import sys


PROJECT_VERSION_FILE_PATH = Path("BetterRocketDesigns") / "BetterRocketDesigns.cs"
ASSEMBLY_INFO_PATH = Path("BetterRocketDesigns/Properties/AssemblyInfo.cs")


def set_assembly_value(new_version: str, file_path: Path, property: str):
    with open(file_path, 'r') as file:
        content = file.read()
        updated_content = re.sub(property + r'\("[^"]+"\)', f'{property}("{new_version}")', content)
        with open(file_path, 'w') as updated_file:
            updated_file.write(updated_content)

    print(f"{file_path} - {property} = {new_version}")


def read_project_version(file_path: Path):
    with open(file_path, 'r') as file:
        content = file.read()
        match = re.search(r'const string VERSION = "([^"]+)"', content)
        if match:
            return match.group(1)
        else:
            raise ValueError(f"Can't find a match file: {file_path}")
        

def write_project_version(file_path: Path, new_version: str):
    with open(file_path, 'r') as file:
        content = file.read()
        updated_content = re.sub(r'const string VERSION = "[^"]+"', f'const string VERSION = "{new_version}"', content, 1)
        with open(file_path, 'w') as updated_file:
            updated_file.write(updated_content)

    print(f"{file_path} - VERSION = {new_version}")


def update_version(new_version: str, new_assembly_version: str):
    set_assembly_value(new_assembly_version, ASSEMBLY_INFO_PATH, "AssemblyVersion")
    set_assembly_value(new_assembly_version, ASSEMBLY_INFO_PATH, "AssemblyFileVersion")

    write_project_version(PROJECT_VERSION_FILE_PATH, new_version)

def short_semver(semver: str):
    match = re.search(r'\d+\.\d+\.\d+', semver)
    if match:
        version = match.group()
        return version
    else:
        raise ValueError(f"Invalid semver format: {semver}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Manage project versions")

    subparsers = parser.add_subparsers(dest="command", help="Available commands")

    get_version_parser = subparsers.add_parser("get-version", help="Print the current version")
    get_version_parser.add_argument("--short", required=False, action="store_true", default=False, help="Print short version")

    update_version_parser = subparsers.add_parser("update-version", help="Update the current version")
    update_version_parser.add_argument("--new-version", required=True, metavar="NEW_VERSION", help="Specify a new version")
    update_version_parser.add_argument("--new-assembly-version", required=True, metavar="NEW_ASSEMBLY_VERSION", help="Specify a new assembly version")

    args = parser.parse_args()

    if args.command == "get-version":
        current_version = read_project_version(PROJECT_VERSION_FILE_PATH)

        if args.short:
            current_version = short_semver(current_version)

        print(current_version)
    elif args.command == "update-version":
        current_version = read_project_version(PROJECT_VERSION_FILE_PATH)
        print(f"Current version: {current_version}")

        update_version(args.new_version, args.new_assembly_version)
    else:
        print("Invalid command. Please use '--help' for more information.")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())