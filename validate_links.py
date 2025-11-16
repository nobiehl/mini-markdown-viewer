#!/usr/bin/env python3
"""
Link validation script for MarkdownViewer documentation.
Finds and validates all markdown links (internal, external, anchors).
"""

import os
import re
import urllib.request
import urllib.error
from pathlib import Path
from typing import Dict, List, Tuple, Set
from dataclasses import dataclass
from collections import defaultdict

@dataclass
class LinkInfo:
    """Information about a found link."""
    file: str
    line: int
    link_text: str
    target: str
    link_type: str  # 'internal', 'external', 'anchor'

class LinkValidator:
    def __init__(self, root_dir: str):
        self.root_dir = Path(root_dir)
        self.links: List[LinkInfo] = []
        self.broken_links: List[Tuple[LinkInfo, str]] = []
        self.valid_links: List[LinkInfo] = []

    def find_all_markdown_files(self) -> List[Path]:
        """Find all .md files in the repository."""
        md_files = []
        for pattern in ["**/*.md", "**/*.MD"]:
            md_files.extend(self.root_dir.glob(pattern))
        return sorted(set(md_files))

    def extract_links(self, file_path: Path) -> List[LinkInfo]:
        """Extract all markdown links from a file."""
        links = []
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.readlines()

            # Pattern: [text](url)
            link_pattern = re.compile(r'\[([^\]]+)\]\(([^)]+)\)')

            for line_num, line in enumerate(content, start=1):
                for match in link_pattern.finditer(line):
                    link_text = match.group(1)
                    target = match.group(2)

                    # Determine link type
                    if target.startswith('data:'):
                        link_type = 'data-uri'
                    elif target.startswith('http://') or target.startswith('https://'):
                        link_type = 'external'
                    elif target.startswith('#'):
                        link_type = 'anchor'
                    elif '#' in target:
                        link_type = 'internal+anchor'
                    else:
                        link_type = 'internal'

                    rel_path = file_path.relative_to(self.root_dir)
                    links.append(LinkInfo(
                        file=str(rel_path),
                        line=line_num,
                        link_text=link_text,
                        target=target,
                        link_type=link_type
                    ))
        except Exception as e:
            print(f"Error reading {file_path}: {e}")

        return links

    def validate_internal_link(self, link: LinkInfo) -> Tuple[bool, str]:
        """Validate an internal file link."""
        source_file = self.root_dir / link.file
        source_dir = source_file.parent

        # Handle anchor part
        target_path = link.target.split('#')[0] if '#' in link.target else link.target

        # Resolve relative path
        if target_path:
            full_path = (source_dir / target_path).resolve()

            # Check if file exists (case-insensitive on Windows)
            if not full_path.exists():
                # Try to find file with different case
                parent = full_path.parent
                if parent.exists():
                    files = list(parent.glob(full_path.name))
                    if not files:
                        return False, f"file not found: {full_path.relative_to(self.root_dir)}"
                else:
                    return False, f"directory not found: {parent.relative_to(self.root_dir)}"

        # Validate anchor if present
        if '#' in link.target:
            anchor = link.target.split('#')[1]
            target_file_path = link.target.split('#')[0]

            if target_file_path:
                full_path = (source_dir / target_file_path).resolve()
            else:
                full_path = source_file

            if full_path.exists():
                is_valid, error = self.validate_anchor(full_path, anchor)
                if not is_valid:
                    return False, error

        return True, ""

    def validate_anchor(self, file_path: Path, anchor: str) -> Tuple[bool, str]:
        """Validate that an anchor exists in a markdown file."""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()

            # Convert anchor to heading format
            # Example: #my-heading → ## My Heading or ### my heading, etc.
            # GitHub/markdown style: lowercase, replace spaces with hyphens

            # Find all headings
            heading_pattern = re.compile(r'^#{1,6}\s+(.+)$', re.MULTILINE)
            headings = heading_pattern.findall(content)

            # Convert headings to anchor format
            valid_anchors = set()
            for heading in headings:
                # Remove markdown formatting
                clean_heading = re.sub(r'\[([^\]]+)\]\([^)]+\)', r'\1', heading)  # Remove links
                clean_heading = re.sub(r'[*_`]', '', clean_heading)  # Remove formatting
                clean_heading = clean_heading.strip()

                # Convert to anchor format (GitHub style)
                anchor_text = clean_heading.lower()
                anchor_text = re.sub(r'[^\w\s-]', '', anchor_text)  # Remove special chars
                anchor_text = re.sub(r'\s+', '-', anchor_text)  # Spaces to hyphens
                anchor_text = re.sub(r'-+', '-', anchor_text)  # Multiple hyphens to one

                valid_anchors.add(anchor_text)

            if anchor.lower() not in valid_anchors:
                return False, f"anchor not found: #{anchor} (available: {', '.join(sorted(valid_anchors)[:5])}...)"

            return True, ""
        except Exception as e:
            return False, f"error reading file for anchor validation: {e}"

    def validate_external_link(self, link: LinkInfo) -> Tuple[bool, str]:
        """Validate an external HTTP(S) link."""
        try:
            # Skip data URIs
            if link.target.startswith('data:'):
                return True, ""

            # Make HEAD request with timeout
            req = urllib.request.Request(link.target, method='HEAD')
            req.add_header('User-Agent', 'MarkdownViewer-LinkValidator/1.0')

            with urllib.request.urlopen(req, timeout=5) as response:
                if response.status == 200:
                    return True, ""
                else:
                    return False, f"HTTP {response.status}"
        except urllib.error.HTTPError as e:
            return False, f"HTTP {e.code}"
        except urllib.error.URLError as e:
            return False, f"timeout or network error: {e.reason}"
        except Exception as e:
            return False, f"error: {str(e)}"

    def validate_all_links(self):
        """Validate all links found in markdown files."""
        print("Finding markdown files...")
        md_files = self.find_all_markdown_files()
        print(f"Found {len(md_files)} markdown files\n")

        print("Extracting links...")
        for md_file in md_files:
            links = self.extract_links(md_file)
            self.links.extend(links)
        print(f"Found {len(self.links)} total links\n")

        # Group links by type
        links_by_type = defaultdict(list)
        for link in self.links:
            links_by_type[link.link_type].append(link)

        print("Validating links...\n")

        # Skip data URIs (always valid)
        print(f"Skipping {len(links_by_type['data-uri'])} data URI links (always valid)...")
        self.valid_links.extend(links_by_type['data-uri'])

        # Validate internal links
        print(f"Validating {len(links_by_type['internal'])} internal links...")
        for link in links_by_type['internal']:
            is_valid, error = self.validate_internal_link(link)
            if is_valid:
                self.valid_links.append(link)
            else:
                self.broken_links.append((link, error))

        # Validate internal+anchor links
        print(f"Validating {len(links_by_type['internal+anchor'])} internal+anchor links...")
        for link in links_by_type['internal+anchor']:
            is_valid, error = self.validate_internal_link(link)
            if is_valid:
                self.valid_links.append(link)
            else:
                self.broken_links.append((link, error))

        # Validate anchor links
        print(f"Validating {len(links_by_type['anchor'])} anchor links...")
        for link in links_by_type['anchor']:
            anchor = link.target[1:]  # Remove leading #
            source_file = self.root_dir / link.file
            is_valid, error = self.validate_anchor(source_file, anchor)
            if is_valid:
                self.valid_links.append(link)
            else:
                self.broken_links.append((link, error))

        # Validate external links
        print(f"Validating {len(links_by_type['external'])} external links...")
        for link in links_by_type['external']:
            is_valid, error = self.validate_external_link(link)
            if is_valid:
                self.valid_links.append(link)
            else:
                self.broken_links.append((link, error))

        print("\n" + "="*80)
        self.print_report()

    def print_report(self):
        """Print validation report."""
        # Count by type
        internal = sum(1 for l in self.links if l.link_type in ['internal', 'internal+anchor'])
        anchors = sum(1 for l in self.links if l.link_type == 'anchor')
        external = sum(1 for l in self.links if l.link_type == 'external')
        data_uris = sum(1 for l in self.links if l.link_type == 'data-uri')

        internal_broken = sum(1 for l, _ in self.broken_links if l.link_type in ['internal', 'internal+anchor'])
        anchors_broken = sum(1 for l, _ in self.broken_links if l.link_type == 'anchor')
        external_broken = sum(1 for l, _ in self.broken_links if l.link_type == 'external')

        print("\n# Link Validation Report\n")
        print("## Summary")
        print(f"- Total links found: {len(self.links)}")
        print(f"- Internal links: {internal} ({internal - internal_broken} passed, {internal_broken} failed)")
        print(f"- External links: {external} ({external - external_broken} passed, {external_broken} failed)")
        print(f"- Anchor links: {anchors} ({anchors - anchors_broken} passed, {anchors_broken} failed)")
        print(f"- Data URIs (images): {data_uris} (all valid)")
        print()

        if self.broken_links:
            print(f"## Broken Links ({len(self.broken_links)})\n")

            # Group by type
            broken_by_type = defaultdict(list)
            for link, error in self.broken_links:
                broken_by_type[link.link_type].append((link, error))

            if broken_by_type.get('internal') or broken_by_type.get('internal+anchor'):
                print(f"### Internal Links ({internal_broken})")
                for link, error in (broken_by_type.get('internal', []) + broken_by_type.get('internal+anchor', [])):
                    print(f"- `{link.file}:{link.line}` -> `{link.target}` ({error})")
                print()

            if broken_by_type.get('external'):
                print(f"### External Links ({external_broken})")
                for link, error in broken_by_type['external']:
                    print(f"- `{link.file}:{link.line}` -> `{link.target}` ({error})")
                print()

            if broken_by_type.get('anchor'):
                print(f"### Anchor Links ({anchors_broken})")
                for link, error in broken_by_type['anchor']:
                    print(f"- `{link.file}:{link.line}` -> `{link.target}` ({error})")
                print()
        else:
            print("## All Links Valid!\n")

        # Sample of valid links
        if self.valid_links:
            print(f"## Valid Links (sample of {min(10, len(self.valid_links))})\n")
            for link in self.valid_links[:10]:
                print(f"- {link.file}:{link.line} -> {link.target} [OK]")

if __name__ == '__main__':
    root_dir = os.path.dirname(os.path.abspath(__file__))
    validator = LinkValidator(root_dir)
    validator.validate_all_links()
