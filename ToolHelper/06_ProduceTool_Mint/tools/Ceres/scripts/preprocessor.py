import os
from pathlib import Path
from collections import defaultdict, deque


def line_history(lines, histlen=3):
    history = deque(maxlen=histlen)
    for lineno, line in enumerate(lines):
        history.append((lineno, line))
        yield line, history


def dirs_endswith(root, suffix):
    for item in Path(root).iterdir():
        if item.is_dir() and not item.name.endswith(suffix):
            yield item


def files_endswith(root, suffix):
    for parent, _, files in os.walk(root):
        for file in files:
            if file.endswith(suffix):
                yield(Path(parent, file))


def search_preprocessor(code_path):
    with open(code_path) as source:
        for lineno, line in enumerate(source):
            if '#if' in line or '#elif' in line:
                yield lineno, line


if __name__ == '__main__':
    ceres_root = r'F:\repo\master\src\sources\dev\common\src\Ceres'
    for folder in dirs_endswith(ceres_root, '.NetCore'):
        for source in files_endswith(folder, '.cs'):
            find = list(search_preprocessor(source))
            if find:
                result = defaultdict(list)
                print(folder.name)
                print(f'  {source}')
                for lineno, line in find:
                    result[line.strip()].append(lineno)
                for recored in result:
                    print(f'    {recored}\n    line: {result[recored]}\n')
